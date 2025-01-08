namespace AssociationRegistry.Vereniging;

using Bronnen;
using EventFactories;
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

        vereniging.AddEvent(new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(verenigingVolgensKbo.KboNummer));

        return vereniging;
    }

    public void SyncCompleted()
    {
        AddEvent(new SynchronisatieMetKboWasSuccesvol(State.KboNummer!));
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

        Throw<LaatsteHoofdActiviteitKanNietVerwijderdWorden>.If(State.HoofdactiviteitenVerenigingsloket.Any() && !hoofdactiviteitenVerenigingsloket.Any());

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(EventFactory.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(hoofdactiviteiten.ToArray()));
    }

    public void WijzigWerkingsgebieden(Werkingsgebied[] werkingsgebieden)
    {
        if (Werkingsgebieden.Equals(werkingsgebieden, State.Werkingsgebieden))
            return;

        var werkingsgebiedenData = Werkingsgebieden.FromArray(werkingsgebieden);
        AddEvent(EventFactory.WerkingsgebiedenWerdenGewijzigd(VCode, werkingsgebiedenData.ToArray()));
    }

    public void WijzigDoelgroep(Doelgroep doelgroep)
    {
        if (Doelgroep.Equals(State.Doelgroep, doelgroep)) return;

        AddEvent(EventFactory.DoelgroepWerdGewijzigd(doelgroep));
    }

    private void VoegMaatschappelijkeZetelToe(Adres adres)
    {
        AddEvent(
            EventFactory.MaatschappelijkeZetelWerdOvergenomenUitKbo(
                Locatie.Create(
                        Locatienaam.Empty,
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

        AddEvent(EventFactory.MaatschappelijkeZetelVolgensKBOWerdGewijzigd(gewijzigdeLocatie));
    }

    public void WijzigMaatschappelijkeZetelUitKbo(AdresVolgensKbo? adresVolgensKbo)
    {
        var maatschappelijkeZetel = State.Locaties.MaatschappelijkeZetel;

        if (maatschappelijkeZetel is null)
        {
            VoegMaatschappelijkeZetelToe(adresVolgensKbo);

            return;
        }

        if (adresVolgensKbo is null || adresVolgensKbo.IsEmpty())
        {
            AddEvent(EventFactory.MaatschappelijkeZetelWerdVerwijderdUitKbo(maatschappelijkeZetel));

            return;
        }

        var adres = Adres.TryCreateFromKbo(adresVolgensKbo);

        if (adres is null)
        {
            AddEvent(EventFactory.MaatschappelijkeZetelWerdVerwijderdUitKbo(maatschappelijkeZetel));
            AddEvent(EventFactory.MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(adresVolgensKbo));

            return;
        }

        if (adres == maatschappelijkeZetel.Adres)
            return;

        var gewijzigdeLocatie = maatschappelijkeZetel.Wijzig(adres: adres);
        AddEvent(EventFactory.MaatschappelijkeZetelWerdGewijzigdInKbo(gewijzigdeLocatie));
    }

    private void VoegContactgegevenToe(Contactgegeven contactgegeven, ContactgegeventypeVolgensKbo typeVolgensKbo)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(EventFactory.ContactgegevenWerdOvergenomenUitKBO(toegevoegdContactgegeven, typeVolgensKbo));
    }

    private void VoegFoutiefContactgegevenToe(ContactgegeventypeVolgensKbo type, string waarde)
    {
        AddEvent(new ContactgegevenKonNietOvergenomenWordenUitKBO(type.Contactgegeventype.Waarde, type.Waarde, waarde));
    }

    private void VoegFoutieveMaatschappelijkeZetelToe(AdresVolgensKbo adres)
    {
        AddEvent(EventFactory.MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(adres));
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

        AddEvent(EventFactory.ContactgegevenUitKBOWerdGewijzigd(gewijzigdContactgegeven));
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

    public void WijzigRechtsvormUitKbo(Verenigingstype verenigingstype)
    {
        if (State.Verenigingstype == verenigingstype) return;
        AddEvent(EventFactory.RechtsvormWerdGewijzigdInKBO(verenigingstype));
    }

    public void WijzigKorteNaamUitKbo(string? korteNaam)
    {
        if (State.KorteNaam == korteNaam) return;
        AddEvent(new KorteNaamWerdGewijzigdInKbo(korteNaam));
    }

    public void WijzigStartdatum(Datum? startdatum)
    {
        if (State.Startdatum == startdatum) return;
        AddEvent(EventFactory.StartdatumWerdGewijzigdInKbo(startdatum));
    }

    public void WijzigContactgegevenUitKbo(string? waarde, ContactgegeventypeVolgensKbo typeVolgensKbo)
    {
        var teWijzigenContactgegeven = State.Contactgegevens.GetContactgegevenOfKboType(typeVolgensKbo);

        var newContactgegeven = waarde is null ? null : Contactgegeven.TryCreateFromKbo(waarde, typeVolgensKbo);

        if (teWijzigenContactgegeven is null)
        {
            if (newContactgegeven is not null)
            {
                var equivalentContactgegeven = State.Contactgegevens.MetZelfdeWaarden(newContactgegeven);

                if (equivalentContactgegeven is not null)
                {
                    AddEvent(EventFactory.ContactgegevenWerdInBeheerGenomenDoorKbo(equivalentContactgegeven, typeVolgensKbo));

                    return;
                }
            }

            VoegContactgegevenToe(waarde, typeVolgensKbo);

            return;
        }

        if (newContactgegeven is null)
        {
            AddEvent(EventFactory.ContactgegevenWerdVerwijderdUitKBO(teWijzigenContactgegeven));

            if (waarde is not null)
                VoegFoutiefContactgegevenToe(typeVolgensKbo, waarde);

            return;
        }

        var gewijzigdContactgegeven = State.Contactgegevens.WijzigUitKbo(teWijzigenContactgegeven.ContactgegevenId, waarde);

        if (gewijzigdContactgegeven is null)
            return;

        AddEvent(EventFactory.ContactgegevenWerdGewijzigdInKbo(gewijzigdContactgegeven, typeVolgensKbo));
    }

    public void StopUitKbo(Datum eindDatum)
    {
        if (State.IsGestopt) return;
        AddEvent(EventFactory.VerenigingWerdGestoptInKBO(eindDatum));
    }

    public void MarkeerAlsIngeschreven(KboNummer kboNummer)
    {
        if(!State.IsIngeschrevenOpWijzigingenUitKbo)
            AddEvent(new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(kboNummer));
    }
}
