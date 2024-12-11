namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using Events;
using Formats;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Schema;
using Schema.Historiek;
using Schema.Historiek.EventData;
using Vereniging;
using IEvent = Marten.Events.IEvent;
using WellknownFormats = Constants.WellknownFormats;

public class BeheerVerenigingHistoriekProjector
{
    public static BeheerVerenigingHistoriekDocument Create(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = CreateNewDocument(feitelijkeVerenigingWerdGeregistreerd.Data.VCode);

        AddHistoriekEntry(
            feitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGeregistreerdData.Create(feitelijkeVerenigingWerdGeregistreerd.Data),
            beheerVerenigingHistoriekDocument,
            $"Feitelijke vereniging werd geregistreerd met naam '{feitelijkeVerenigingWerdGeregistreerd.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static BeheerVerenigingHistoriekDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = CreateNewDocument(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode);

        AddHistoriekEntry(
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            beheerVerenigingHistoriekDocument,
            $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            document,
            $"Naam werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}'.");

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            roepnaamWerdGewijzigd,
            document,
            $"Roepnaam werd gewijzigd naar '{roepnaamWerdGewijzigd.Data.Roepnaam}'.");

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            document,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.");

    public static void Apply(
        IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            document,
            $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.");

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        if (startdatumWerdGewijzigd.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);

            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd gewijzigd naar '{startDatumString}'."
            );
        }
        else
        {
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                beschrijving: "Startdatum werd verwijderd."
            );
        }
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, BeheerVerenigingHistoriekDocument document)
    {
        if (startdatumWerdGewijzigdInKbo.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);

            AddHistoriekEntry(
                startdatumWerdGewijzigdInKbo,
                document,
                $"In KBO werd de startdatum gewijzigd naar '{startDatumString}'."
            );
        }
        else
        {
            AddHistoriekEntry(
                startdatumWerdGewijzigdInKbo,
                document,
                beschrijving: "In KBO werd de startdatum verwijderd."
            );
        }
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            doelgroepWerdGewijzigd,
            document,
            $"Doelgroep werd gewijzigd naar '{doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd} " +
            $"- {doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd}'.");

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
            document,
            beschrijving: "Hoofdactiviteiten verenigingsloket werden gewijzigd.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietBepaald> werkingsgebiedenWerdenNietBepaald,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            werkingsgebiedenWerdenNietBepaald,
            document,
            beschrijving: "Werkingsgebieden werden niet bepaald.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenBepaald> werkingsgebiedenWerdenBepaald,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            werkingsgebiedenWerdenBepaald,
            document,
            beschrijving: "Werkingsgebieden werden bepaald.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenGewijzigd> werkingsgebiedenWerdenGewijzigd,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            werkingsgebiedenWerdenGewijzigd,
            document,
            beschrijving: "Werkingsgebieden werden gewijzigd.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietVanToepassing> werkingsgebiedenWerdenNietVanToepassing,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            werkingsgebiedenWerdenNietVanToepassing,
            document,
            beschrijving: "Werkingsgebieden werden niet van toepassing.");

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdToegevoegd,
            document,
            $"{contactgegevenWerdToegevoegd.Data.Contactgegeventype} '{contactgegevenWerdToegevoegd.Data.Waarde}' werd toegevoegd."
        );
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdVerwijderd,
            document,
            $"{contactgegevenWerdVerwijderd.Data.Type} '{contactgegevenWerdVerwijderd.Data.Waarde}' werd verwijderd."
        );
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdGewijzigd,
            document,
            $"{contactgegevenWerdGewijzigd.Data.Contactgegeventype} '{contactgegevenWerdGewijzigd.Data.Waarde}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdToegevoegd,
            VertegenwoordigerData.Create(vertegenwoordigerWerdToegevoegd.Data),
            document,
            $"'{vertegenwoordigerWerdToegevoegd.Data.Voornaam} {vertegenwoordigerWerdToegevoegd.Data.Achternaam}' werd toegevoegd als vertegenwoordiger."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdGewijzigd,
            VertegenwoordigerData.Create(vertegenwoordigerWerdGewijzigd.Data),
            document,
            $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdVerwijderd,
            VertegenwoordigerWerdVerwijderdData.Create(vertegenwoordigerWerdVerwijderd.Data),
            document,
            $"Vertegenwoordiger '{vertegenwoordigerWerdVerwijderd.Data.Voornaam} {vertegenwoordigerWerdVerwijderd.Data.Achternaam}' werd verwijderd."
        );
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekeDatastroom,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdUitgeschrevenUitPubliekeDatastroom,
            verenigingWerdUitgeschrevenUitPubliekeDatastroom.Data,
            document,
            beschrijving: "Vereniging werd uitgeschreven uit de publieke datastroom."
        );
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdIngeschrevenInPubliekeDatastroom,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdIngeschrevenInPubliekeDatastroom,
            verenigingWerdIngeschrevenInPubliekeDatastroom.Data,
            document,
            beschrijving: "Vereniging werd ingeschreven in de publieke datastroom."
        );
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(locatieWerdToegevoegd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdToegevoegd.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            locatieWerdToegevoegd,
            locatieWerdToegevoegd.Data.Locatie,
            document,
            $"'{locatieWerdToegevoegd.Data.Locatie.Locatietype}' locatie {naam}werd toegevoegd."
        );
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(locatieWerdGewijzigd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdGewijzigd.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            locatieWerdGewijzigd,
            locatieWerdGewijzigd.Data.Locatie,
            document,
            $"'{locatieWerdGewijzigd.Data.Locatie.Locatietype}' locatie {naam}werd gewijzigd."
        );
    }

    public static void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(locatieWerdVerwijderd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdVerwijderd.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            locatieWerdVerwijderd,
            locatieWerdVerwijderd.Data.Locatie,
            document,
            $"'{locatieWerdVerwijderd.Data.Locatie.Locatietype}' locatie {naam}werd verwijderd."
        );
    }

    private static void AddHistoriekEntry(IEvent @event, BeheerVerenigingHistoriekDocument document, string beschrijving)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsZuluTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                @event.Data,
                initiator,
                tijdstip
            )).ToList();
    }

    private static void AddHistoriekEntry(IEvent @event, object data, BeheerVerenigingHistoriekDocument document, string beschrijving)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsZuluTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                data,
                initiator,
                tijdstip
            )).ToList();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            maatschappelijkeZetelWerdOvergenomenUitKbo,
            maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd overgenomen uit KBO."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> maatschappelijkeZetelWerdGewijzigdInKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            maatschappelijkeZetelWerdGewijzigdInKbo,
            maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd gewijzigd in KBO."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> maatschappelijkeZetelWerdVerwijderdUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            maatschappelijkeZetelWerdVerwijderdUitKbo,
            maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd verwijderd uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenUitKBOWerdGewijzigd> contactgegevenUitKboWerdGewijzigd,
        BeheerVerenigingHistoriekDocument document)
    {
        var contactgegevenWerdOvergenomenUitKbo = document.Gebeurtenissen
                                                          .Where(x => x.Gebeurtenis == nameof(ContactgegevenWerdOvergenomenUitKBO))
                                                          .Select(x => (ContactgegevenWerdOvergenomenUitKBO)x.Data!)
                                                          .Single(x => x.ContactgegevenId ==
                                                                       contactgegevenUitKboWerdGewijzigd.Data.ContactgegevenId);

        var type = contactgegevenWerdOvergenomenUitKbo.TypeVolgensKbo;
        var waarde = contactgegevenWerdOvergenomenUitKbo.Waarde;

        AddHistoriekEntry(
            contactgegevenUitKboWerdGewijzigd,
            contactgegevenUitKboWerdGewijzigd.Data,
            document,
            $"{type} '{waarde}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> maatschappelijkeZetelVolgensKboWerdGewijzigd,
        BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam)
            ? string.Empty
            : $"'{maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam}' ";

        AddHistoriekEntry(
            maatschappelijkeZetelVolgensKboWerdGewijzigd,
            maatschappelijkeZetelVolgensKboWerdGewijzigd.Data,
            document,
            beschrijving: "Maatschappelijke zetel volgens KBO werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            maatschappelijkeZetelWerdOvergenomenUitKbo,
            maatschappelijkeZetelWerdOvergenomenUitKbo.Data,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO’ kon niet overgenomen worden uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdOvergenomen,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdOvergenomen,
            document,
            $"Contactgegeven ‘{contactgegevenWerdOvergenomen.Data.TypeVolgensKbo}' werd overgenomen uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> contactgegevenKonNietOvergenomenWorden,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenKonNietOvergenomenWorden,
            document,
            $"Contactgegeven ‘{contactgegevenKonNietOvergenomenWorden.Data.TypeVolgensKbo}' kon niet overgenomen worden uit KBO."
        );
    }

    public static void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdGestopt,
            document,
            $"De vereniging werd gestopt met einddatum '{verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdGestoptInKbo,
            document,
            $"De vereniging werd gestopt in KBO met einddatum '{verenigingWerdGestoptInKbo.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            einddatumWerdGewijzigd,
            document,
            $"De einddatum van de vereniging werd gewijzigd naar '{einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdOvergenomenUitKbo,
            VertegenwoordigerData.Create(vertegenwoordigerWerdOvergenomenUitKbo.Data),
            document,
            $"Vertegenwoordiger '{vertegenwoordigerWerdOvergenomenUitKbo.Data.Voornaam} {vertegenwoordigerWerdOvergenomenUitKbo.Data.Achternaam}' werd overgenomen uit KBO."
        );
    }

    public static void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            naamWerdGewijzigdInKbo,
            document,
            $"In KBO werd de naam gewijzigd naar '{naamWerdGewijzigdInKbo.Data.Naam}'."
        );
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            korteNaamWerdGewijzigdInKbo,
            document,
            $"In KBO werd de korte naam gewijzigd naar '{korteNaamWerdGewijzigdInKbo.Data.KorteNaam}'."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> contactgegevenWerdGewijzigdUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdGewijzigdUitKbo,
            document,
            $"In KBO werd contactgegeven ‘{contactgegevenWerdGewijzigdUitKbo.Data.TypeVolgensKbo}' gewijzigd."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> contactgegevenWerdVerwijderdUitKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdVerwijderdUitKbo,
            document,
            $"In KBO werd contactgegeven ‘{contactgegevenWerdVerwijderdUitKbo.Data.TypeVolgensKbo}' verwijderd."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> contactgegevenWerdInBeheerGenomenDoorKbo,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdInBeheerGenomenDoorKbo,
            document,
            $"{contactgegevenWerdInBeheerGenomenDoorKbo.Data.Contactgegeventype} '{contactgegevenWerdInBeheerGenomenDoorKbo.Data.Waarde}' werd in beheer genomen door KBO."
        );
    }

    public static void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            rechtsvormWerdGewijzigdInKbo,
            document,
            $"In KBO werd de rechtsvorm gewijzigd naar '{Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam}'."
        );
    }

    public static void UpdateMetadata(IEvent @event, BeheerVerenigingHistoriekDocument document)
    {
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    public static void Apply(IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>();

        AddHistoriekEntry(
            verenigingWerdVerwijderd,
            new VerenigingWerdVerwijderdData(
                verenigingWerdVerwijderd.Data.Reden
            ),
            document,
            beschrijving: "Deze vereniging werd verwijderd."
        );
    }

    public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresWerdOvergenomenUitAdressenregister,
            document,
            $"Adres werd overgenomen uit het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdGewijzigdInAdressenregister> adresWerdGewijzigdInAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresWerdGewijzigdInAdressenregister,
            document,
            $"Adres werd gewijzigd in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> adresKonNietOvergenomenWordenUitAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresKonNietOvergenomenWordenUitAdressenregister,
            document,
            $"Adres kon niet overgenomen worden uit het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresNietUniekInAdressenregister> adresNietUniekInAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresNietUniekInAdressenregister,
            document,
            $"Adres niet uniek in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> adresWerdNietGevondenInAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresWerdNietGevondenInAdressenregister,
            document,
            $"Adres kon niet gevonden worden in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdOntkoppeldVanAdressenregister> adresWerdOntkoppeldVanAdressenregister,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            adresWerdOntkoppeldVanAdressenregister,
            document,
            $"Adres werd ontkoppeld van het adressenregister."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdToegevoegd> lidmaatschapWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            lidmaatschapWerdToegevoegd,
            LidmaatschapData.Create(lidmaatschapWerdToegevoegd.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd toegevoegd."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdGewijzigd> lidmaatschapWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            lidmaatschapWerdGewijzigd,
            LidmaatschapData.Create(lidmaatschapWerdGewijzigd.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd gewijzigd."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdVerwijderd> lidmaatschapWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            lidmaatschapWerdVerwijderd,
            LidmaatschapData.Create(lidmaatschapWerdVerwijderd.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd verwijderd."
        );
    }

    public static void Apply(IEvent<VerenigingWerdGermarkeerdAlsDubbelVan> verenigingWerdGemarkeerdAlsDubbelVan, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdGemarkeerdAlsDubbelVan,
            new
            {
                VCode = verenigingWerdGemarkeerdAlsDubbelVan.Data.VCode,
                VCodeAuthentiekeVereniging = verenigingWerdGemarkeerdAlsDubbelVan.Data.VCodeAuthentiekeVereniging,
            },
            document,
            $"Vereniging werd gemarkeerd als dubbel van {verenigingWerdGemarkeerdAlsDubbelVan.Data.VCodeAuthentiekeVereniging}."
        );
    }

    private static BeheerVerenigingHistoriekDocument CreateNewDocument(string vCode)
        => new()
        {
            VCode = vCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

    public static void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> locatieDuplicaatWerdVerwijderd,
        BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(locatieDuplicaatWerdVerwijderd.Data.LocatieNaam)
            ? string.Empty
            : locatieDuplicaatWerdVerwijderd.Data.LocatieNaam;

        AddHistoriekEntry(
            locatieDuplicaatWerdVerwijderd,
            document,
            $"Locatie '{naam}' met ID {locatieDuplicaatWerdVerwijderd.Data.VerwijderdeLocatieId} werd verwijderd omdat de gegevens exact overeenkomen met locatie ID {locatieDuplicaatWerdVerwijderd.Data.BehoudenLocatieId}.");
    }
}
