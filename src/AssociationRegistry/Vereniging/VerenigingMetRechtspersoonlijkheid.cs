namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;
using Kbo;

public class VerenigingMetRechtspersoonlijkheid : VerenigingsBase, IHydrate<VerenigingState>
{
    private static Verenigingstype[] _allowedTypes;

    public static VerenigingMetRechtspersoonlijkheid Registreer(VCode vCode, VerenigingVolgensKbo verenigingVolgensKbo)
    {
        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.AddEvent(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                verenigingVolgensKbo.KboNummer,
                verenigingVolgensKbo.Type.Code,
                verenigingVolgensKbo.Naam ?? "",
                verenigingVolgensKbo.KorteNaam ?? "",
                verenigingVolgensKbo.StartDatum ?? null));

        vereniging.VoegMaatschappelijkeZetelToe(verenigingVolgensKbo.Adres);

        if (verenigingVolgensKbo.Contactgegevens is not null) // TODO: question: is this only for test purposes?
        {
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Email, ContactgegevenTypeVolgensKbo.Email);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Website, ContactgegevenTypeVolgensKbo.Website);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Telefoonnummer, ContactgegevenTypeVolgensKbo.Telefoon);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.GSM, ContactgegevenTypeVolgensKbo.GSM);
        }

        vereniging.VoegVertegenwoordigersToe(verenigingVolgensKbo.Vertegenwoordigers);

        return vereniging;
    }

    private void VoegVertegenwoordigersToe(VertegenwoordigerVolgensKbo[] vertegenwoordigers)
    {
        foreach (var (vertegenwoordiger, id) in vertegenwoordigers.Select((x, i) => (x, i)))
        {
            AddEvent(
                new VertegenwoordigerWerdOvergenomenUitKBO(
                    id + 1,
                    vertegenwoordiger.Insz,
                    vertegenwoordiger.Voornaam,
                    vertegenwoordiger.Achternaam));
        }
    }

    public void WijzigRoepnaam(string roepnaam)
    {
        if (roepnaam.Equals(State.Roepnaam))
            return;

        AddEvent(new RoepnaamWerdGewijzigd(roepnaam));
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(State.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
    }

    public void WijzigDoelgroep(Doelgroep doelgroep)
    {
        if (Doelgroep.Equals(State.Doelgroep, doelgroep)) return;

        AddEvent(DoelgroepWerdGewijzigd.With(doelgroep));
    }

    private void VoegMaatschappelijkeZetelToe(Adres adres)
    {
        AddEvent(
            MaatschappelijkeZetelWerdOvergenomenUitKbo.With(
                Locatie.Create(
                        string.Empty,
                        isPrimair: false,
                        Locatietype.MaatschappelijkeZetelVolgensKbo,
                        adresId: null,
                        adres)
                    with
                    {
                        LocatieId = State.Locaties.NextId,
                    }
            )
        );
    }

    private void VoegContactgegevenToe(Contactgegeven contactgegeven, ContactgegevenTypeVolgensKbo typeVolgensKbo)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(ContactgegevenWerdOvergenomenUitKBO.With(toegevoegdContactgegeven, typeVolgensKbo));
    }

    private void VoegFoutiefContactgegevenToe(ContactgegevenTypeVolgensKbo type, string waarde)
    {
        AddEvent(new ContactgegevenKonNietOvergenomenWordenUitKBO(type.ContactgegevenType.Waarde, type.Waarde, waarde));
    }

    private void VoegFoutieveMaatscheppelijkeZetelToe(AdresVolgensKbo adres)
    {
        AddEvent(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo.With(adres));
    }

    private void VoegContactgegevenToe(string? waarde, ContactgegevenTypeVolgensKbo type)
    {
        if (waarde is null) return;

        var contactgegeven = Contactgegeven.TryCreateFromKbo(waarde, type);

        if (contactgegeven is null)
        {
            VoegFoutiefContactgegevenToe(type, waarde);

            return;
        }

        VoegContactgegevenToe(contactgegeven, type);
    }

    private void VoegMaatschappelijkeZetelToe(AdresVolgensKbo? adresVolgensKbo)
    {
        if (adresVolgensKbo is null || adresVolgensKbo.IsEmpty()) return;

        var adres = Adres.TryCreateFromKbo(adresVolgensKbo);

        if (adres is null)
        {
            VoegFoutieveMaatscheppelijkeZetelToe(adresVolgensKbo);

            return;
        }

        VoegMaatschappelijkeZetelToe(adres);
    }

    public void Hydrate(VerenigingState obj)
    {
        _allowedTypes = new[]
        {
            Verenigingstype.VZW,
            Verenigingstype.IVZW,
            Verenigingstype.PrivateStichting,
            Verenigingstype.StichtingVanOpenbaarNut,
        };

        Throw<UnsupportedOperationForVerenigingstype>.If(
            !_allowedTypes.Contains(obj.Verenigingstype));

        State = obj;
    }
}
