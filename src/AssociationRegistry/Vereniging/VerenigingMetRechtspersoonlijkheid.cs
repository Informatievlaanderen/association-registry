namespace AssociationRegistry.Vereniging;

using Bronnen;
using Events;
using Exceptions;
using Framework;
using Kbo;

public class VerenigingMetRechtspersoonlijkheid : VerenigingsBase, IHydrate<VerenigingState>
{
    private static Verenigingstype[] _allowedTypes;

    public void Hydrate(VerenigingState obj)
    {
        _allowedTypes = new[]
        {
            Verenigingstype.VZW,
            Verenigingstype.IVZW,
            Verenigingstype.PrivateStichting,
            Verenigingstype.StichtingVanOpenbaarNut,
        };

        Throw<ActieIsNietToegestaanVoorVerenigingstype>.If(
            !_allowedTypes.Contains(obj.Verenigingstype));

        State = obj;
    }

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
                verenigingVolgensKbo.Startdatum ?? null));

        vereniging.VoegMaatschappelijkeZetelToe(verenigingVolgensKbo.Adres);

        if (verenigingVolgensKbo.Contactgegevens is not null) // TODO: question: is this only for test purposes?
        {
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Email, ContactgegeventypeVolgensKbo.Email);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Website, ContactgegeventypeVolgensKbo.Website);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.Telefoonnummer, ContactgegeventypeVolgensKbo.Telefoon);
            vereniging.VoegContactgegevenToe(verenigingVolgensKbo.Contactgegevens.GSM, ContactgegeventypeVolgensKbo.GSM);
        }

        vereniging.VoegVertegenwoordigersToe(verenigingVolgensKbo.Vertegenwoordigers);

        return vereniging;
    }

    public void SyncCompleted()
    {
        AddEvent(new KboSyncSuccessful());
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

    public void WijzigMaatschappelijkeZetel(int locatieId, string? naam, bool? isPrimair)
    {
        var gewijzigdeLocatie = State.Locaties.Wijzig(locatieId, naam, isPrimair);

        if (gewijzigdeLocatie is null)
            return;

        Throw<ActieIsNietToegestaanVoorLocatieType>.If(gewijzigdeLocatie.Locatietype != Locatietype.MaatschappelijkeZetelVolgensKbo);

        AddEvent(MaatschappelijkeZetelVolgensKBOWerdGewijzigd.With(gewijzigdeLocatie));
    }

    private void VoegContactgegevenToe(Contactgegeven contactgegeven, ContactgegeventypeVolgensKbo typeVolgensKbo)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(ContactgegevenWerdOvergenomenUitKBO.With(toegevoegdContactgegeven, typeVolgensKbo));
    }

    private void VoegFoutiefContactgegevenToe(ContactgegeventypeVolgensKbo type, string waarde)
    {
        AddEvent(new ContactgegevenKonNietOvergenomenWordenUitKBO(type.Contactgegeventype.Waarde, type.Waarde, waarde));
    }

    private void VoegFoutieveMaatschappelijkeZetelToe(AdresVolgensKbo adres)
    {
        AddEvent(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo.With(adres));
    }

    private void VoegContactgegevenToe(string? waarde, ContactgegeventypeVolgensKbo type)
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

    public void WijzigContactgegeven(int contactgegevenId, string? beschrijving, bool? isPrimair)
    {
        var gewijzigdContactgegeven = State.Contactgegevens.Wijzig(contactgegevenId, beschrijving, isPrimair);

        if (gewijzigdContactgegeven is null)
            return;

        Throw<ActieIsNietToegestaanVoorContactgegevenBron>.If(gewijzigdContactgegeven.Bron != Bron.KBO);

        AddEvent(ContactgegevenUitKBOWerdGewijzigd.With(gewijzigdContactgegeven));
    }

    private void VoegMaatschappelijkeZetelToe(AdresVolgensKbo? adresVolgensKbo)
    {
        if (adresVolgensKbo is null || adresVolgensKbo.IsEmpty()) return;

        var adres = Adres.TryCreateFromKbo(adresVolgensKbo);

        if (adres is null)
        {
            VoegFoutieveMaatschappelijkeZetelToe(adresVolgensKbo);

            return;
        }

        VoegMaatschappelijkeZetelToe(adres);
    }

    public void WijzigNaamUitKbo(VerenigingsNaam naam)
    {
        if (State.Naam == naam) return;
        AddEvent(new NaamWerdGewijzigdInKbo(naam));
    }

    public void WijzigKorteNaamUitKbo(string? korteNaam)
    {
        if (State.KorteNaam == korteNaam) return;
        AddEvent(new KorteNaamWerdGewijzigdInKbo(korteNaam));
    }

    public void WijzigContactgegevensUitKbo(ContactgegevensVolgensKbo contactgegevens)
    {
        WijzigContactgegevenUitKbo(contactgegevens.Email, ContactgegeventypeVolgensKbo.Email);
        WijzigContactgegevenUitKbo(contactgegevens.Website, ContactgegeventypeVolgensKbo.Website);
        WijzigContactgegevenUitKbo(contactgegevens.Telefoonnummer, ContactgegeventypeVolgensKbo.Telefoon);
        WijzigContactgegevenUitKbo(contactgegevens.GSM, ContactgegeventypeVolgensKbo.GSM);
    }

    private void WijzigContactgegevenUitKbo(string? waarde, ContactgegeventypeVolgensKbo typeVolgensKbo)
    {
        var gewijzigdContactgegeven = State.Contactgegevens.WijzigUitKbo(waarde, typeVolgensKbo);

        if (gewijzigdContactgegeven is null)
            return;

        AddEvent(ContactgegevenWerdGewijzigdInKbo.With(gewijzigdContactgegeven, typeVolgensKbo));
    }
}
