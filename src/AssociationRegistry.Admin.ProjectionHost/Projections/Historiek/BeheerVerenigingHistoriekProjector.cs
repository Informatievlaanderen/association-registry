namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using Events;
using Formats;
using Framework;
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
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event)
    {
        var beheerVerenigingHistoriekDocument = CreateNewDocument(@event.Data.VCode);

        AddHistoriekEntry(
            @event,
            VerenigingWerdGeregistreerdData.Create(@event.Data),
            beheerVerenigingHistoriekDocument,
            $"Feitelijke vereniging werd geregistreerd met naam '{@event.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static BeheerVerenigingHistoriekDocument Create(
        IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> @event)
    {
        var beheerVerenigingHistoriekDocument = CreateNewDocument(@event.Data.VCode);

        AddHistoriekEntry(
            @event,
            VerenigingWerdGeregistreerdData.Create(@event.Data),
            beheerVerenigingHistoriekDocument,
            $"Vereniging zonder eigen rechtspersoonlijkheid werd geregistreerd met naam '{@event.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static BeheerVerenigingHistoriekDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event)
    {
        var beheerVerenigingHistoriekDocument = CreateNewDocument(@event.Data.VCode);

        AddHistoriekEntry(
            @event,
            beheerVerenigingHistoriekDocument,
            $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{@event.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static void Apply(IEvent<NaamWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            $"Naam werd gewijzigd naar '{@event.Data.Naam}'.");

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            $"Roepnaam werd gewijzigd naar '{@event.Data.Roepnaam}'.");

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            $"Korte naam werd gewijzigd naar '{@event.Data.KorteNaam}'.");

    public static void Apply(
        IEvent<KorteBeschrijvingWerdGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            $"Korte beschrijving werd gewijzigd naar '{@event.Data.KorteBeschrijving}'.");

    public static void Apply(IEvent<StartdatumWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        if (@event.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);

            AddHistoriekEntry(
                @event,
                document,
                $"Startdatum werd gewijzigd naar '{startDatumString}'."
            );
        }
        else
        {
            AddHistoriekEntry(
                @event,
                document,
                beschrijving: "Startdatum werd verwijderd."
            );
        }
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigdInKbo> @event, BeheerVerenigingHistoriekDocument document)
    {
        if (@event.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);

            AddHistoriekEntry(
                @event,
                document,
                $"In KBO werd de startdatum gewijzigd naar '{startDatumString}'."
            );
        }
        else
        {
            AddHistoriekEntry(
                @event,
                document,
                beschrijving: "In KBO werd de startdatum verwijderd."
            );
        }
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            $"Doelgroep werd gewijzigd naar '{@event.Data.Doelgroep.Minimumleeftijd} " +
            $"- {@event.Data.Doelgroep.Maximumleeftijd}'.");

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            beschrijving: "Hoofdactiviteiten verenigingsloket werden gewijzigd.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietBepaald> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            beschrijving: "Werkingsgebieden werden niet bepaald.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenBepaald> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            beschrijving: "Werkingsgebieden werden bepaald.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            beschrijving: "Werkingsgebieden werden gewijzigd.");

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietVanToepassing> @event,
        BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            @event,
            document,
            beschrijving: "Werkingsgebieden werden niet van toepassing.");

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"{@event.Data.Contactgegeventype} '{@event.Data.Waarde}' werd toegevoegd."
        );
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"{@event.Data.Type} '{@event.Data.Waarde}' werd verwijderd."
        );
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"{@event.Data.Contactgegeventype} '{@event.Data.Waarde}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            VertegenwoordigerData.Create(@event.Data),
            document,
            $"'{@event.Data.Voornaam} {@event.Data.Achternaam}' werd toegevoegd als vertegenwoordiger."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            VertegenwoordigerData.Create(@event.Data),
            document,
            $"Vertegenwoordiger '{@event.Data.Voornaam} {@event.Data.Achternaam}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdVerwijderd> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            VertegenwoordigerWerdVerwijderdData.Create(@event.Data),
            document,
            $"Vertegenwoordiger '{@event.Data.Voornaam} {@event.Data.Achternaam}' werd verwijderd."
        );
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            beschrijving: "Vereniging werd uitgeschreven uit de publieke datastroom."
        );
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            beschrijving: "Vereniging werd ingeschreven in de publieke datastroom."
        );
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> @event, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(@event.Data.Locatie.Naam)
            ? string.Empty
            : $"'{@event.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            $"'{@event.Data.Locatie.Locatietype}' locatie {naam}werd toegevoegd."
        );
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(@event.Data.Locatie.Naam)
            ? string.Empty
            : $"'{@event.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            $"'{@event.Data.Locatie.Locatietype}' locatie {naam}werd gewijzigd."
        );
    }

    public static void Apply(IEvent<LocatieWerdVerwijderd> @event, BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(@event.Data.Locatie.Naam)
            ? string.Empty
            : $"'{@event.Data.Locatie.Naam}' ";

        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            $"'{@event.Data.Locatie.Locatietype}' locatie {naam}werd verwijderd."
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
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd overgenomen uit KBO."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd gewijzigd in KBO."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data.Locatie,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd verwijderd uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenUitKBOWerdGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        var contactgegevenWerdOvergenomenUitKbo = document.Gebeurtenissen
                                                          .Where(x => x.Gebeurtenis == nameof(ContactgegevenWerdOvergenomenUitKBO))
                                                          .Select(x => (ContactgegevenWerdOvergenomenUitKBO)x.Data!)
                                                          .Single(x => x.ContactgegevenId ==
                                                                       @event.Data.ContactgegevenId);

        var type = contactgegevenWerdOvergenomenUitKbo.TypeVolgensKbo;
        var waarde = contactgegevenWerdOvergenomenUitKbo.Waarde;

        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"{type} '{waarde}' werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(@event.Data.Naam)
            ? string.Empty
            : $"'{@event.Data.Naam}' ";

        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            beschrijving: "Maatschappelijke zetel volgens KBO werd gewijzigd."
        );
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO’ kon niet overgenomen worden uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Contactgegeven ‘{@event.Data.TypeVolgensKbo}' werd overgenomen uit KBO."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Contactgegeven ‘{@event.Data.TypeVolgensKbo}' kon niet overgenomen worden uit KBO."
        );
    }

    public static void Apply(IEvent<VerenigingWerdGestopt> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"De vereniging werd gestopt met einddatum '{@event.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(IEvent<VerenigingWerdGestoptInKBO> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"De vereniging werd gestopt in KBO met einddatum '{@event.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(IEvent<EinddatumWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"De einddatum van de vereniging werd gewijzigd naar '{@event.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'."
        );
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            VertegenwoordigerData.Create(@event.Data),
            document,
            $"Vertegenwoordiger '{@event.Data.Voornaam} {@event.Data.Achternaam}' werd overgenomen uit KBO."
        );
    }

    public static void Apply(IEvent<NaamWerdGewijzigdInKbo> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"In KBO werd de naam gewijzigd naar '{@event.Data.Naam}'."
        );
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"In KBO werd de korte naam gewijzigd naar '{@event.Data.KorteNaam}'."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"In KBO werd contactgegeven ‘{@event.Data.TypeVolgensKbo}' gewijzigd."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"In KBO werd contactgegeven ‘{@event.Data.TypeVolgensKbo}' verwijderd."
        );
    }

    public static void Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"{@event.Data.Contactgegeventype} '{@event.Data.Waarde}' werd in beheer genomen door KBO."
        );
    }

    public static void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"In KBO werd de rechtsvorm gewijzigd naar '{Verenigingstype.Parse(@event.Data.Rechtsvorm).Naam}'."
        );
    }

    public static void UpdateMetadata(IEvent @event, BeheerVerenigingHistoriekDocument document)
    {
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    public static void Apply(IEvent<VerenigingWerdVerwijderd> @event, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>();

        AddHistoriekEntry(
            @event,
            new VerenigingWerdVerwijderdData(
                @event.Data.Reden
            ),
            document,
            beschrijving: "Deze vereniging werd verwijderd."
        );
    }

    public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres werd overgenomen uit het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdGewijzigdInAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres werd gewijzigd in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres kon niet overgenomen worden uit het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresNietUniekInAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres niet uniek in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres kon niet gevonden worden in het adressenregister."
        );
    }

    public static void Apply(
        IEvent<AdresWerdOntkoppeldVanAdressenregister> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            document,
            $"Adres werd ontkoppeld van het adressenregister."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdToegevoegd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            LidmaatschapData.Create(@event.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd toegevoegd."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            LidmaatschapData.Create(@event.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd gewijzigd."
        );
    }

    public static void Apply(IEvent<LidmaatschapWerdVerwijderd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            LidmaatschapData.Create(@event.Data.Lidmaatschap),
            document,
            "Lidmaatschap werd verwijderd."
        );
    }

    public static void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> @event,
        BeheerVerenigingHistoriekDocument document)
    {
        var naam = string.IsNullOrEmpty(@event.Data.LocatieNaam)
            ? string.Empty
            : @event.Data.LocatieNaam;

        AddHistoriekEntry(
            @event,
            document,
            $"Locatie '{naam}' met ID {@event.Data.VerwijderdeLocatieId} werd verwijderd omdat de gegevens exact overeenkomen met locatie ID {@event.Data.BehoudenLocatieId}.");
    }

    public static void Apply(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"Vereniging werd gemarkeerd als dubbel van {@event.Data.VCodeAuthentiekeVereniging}."
        );
    }

    public static void Apply(IEvent<VerenigingAanvaarddeDubbeleVereniging> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"Vereniging aanvaardde dubbele vereniging {@event.Data.VCodeDubbeleVereniging}."
        );
    }

    public static void Apply(IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"Vereniging is geen dubbel meer van {@event.Data.VCodeAuthentiekeVereniging}."
        );
    }

    public static void Apply(IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"Markering als dubbel van vereniging {@event.Data.VCodeAuthentiekeVereniging} werd gecorrigeerd."
        );
    }

    public static void Apply(IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            $"Vereniging {@event.Data.VCodeDubbeleVereniging} werd verwijderd als dubbel door correctie."
        );
    }

    public static void Apply(IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "Feitelijke vereniging werd gemigreerd naar vereniging zonder eigen rechtspersoonlijkheid."
        );
    }


    public static void Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "Subtype werd verfijnd naar feitelijke vereniging."
        );
    }


    public static void Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "Subtype werd verfijnd naar subvereniging."
        );
    }

    public static void Apply(IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "Subtype werd teruggezet naar niet bepaald."
        );
    }

    public static void Apply(IEvent<SubverenigingRelatieWerdGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "De relatie van het subtype werd gewijzigd."
        );
    }

    public static void Apply(IEvent<SubverenigingDetailsWerdenGewijzigd> @event, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            @event,
            @event.Data,
            document,
            "De details van het subtype werden gewijzigd."
        );
    }


    private static BeheerVerenigingHistoriekDocument CreateNewDocument(string vCode)
        => new()
        {
            VCode = vCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };
}
