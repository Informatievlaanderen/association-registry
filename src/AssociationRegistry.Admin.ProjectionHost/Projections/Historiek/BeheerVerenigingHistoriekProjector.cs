namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using System.Collections.Generic;
using Constants;
using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Schema;
using Schema.Historiek;
using Schema.Historiek.EventData;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingHistoriekProjector
{
    public static BeheerVerenigingHistoriekDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

        AddHistoriekEntry(
            feitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGeregistreerdData.Create(feitelijkeVerenigingWerdGeregistreerd.Data),
            beheerVerenigingHistoriekDocument,
            $"Feitelijke vereniging werd geregistreerd met naam '{feitelijkeVerenigingWerdGeregistreerd.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static BeheerVerenigingHistoriekDocument Create(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = afdelingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

        AddHistoriekEntry(
            afdelingWerdGeregistreerd,
            AfdelingWerdGeregistreerdData.Create(afdelingWerdGeregistreerd.Data),
            beheerVerenigingHistoriekDocument,
            $"Afdeling werd geregistreerd met naam '{afdelingWerdGeregistreerd.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public static void Apply(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, BeheerVerenigingHistoriekDocument moeder)
    {
        AddHistoriekEntry(
            afdelingWerdGeregistreerd,
            AfdelingWerdGeregistreerdData.Create(afdelingWerdGeregistreerd.Data),
            moeder,
            $"'{afdelingWerdGeregistreerd.Data.Naam}' werd geregistreerd als afdeling.");
    }

    public static BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

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

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            document,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.");

    public static void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
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
                "Startdatum werd verwijderd."
            );
        }
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            doelgroepWerdGewijzigd,
            document,
            $"Doelgroep werd gewijzigd naar '{doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd} " +
            $"- {doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd}'.");

    public static void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
            document,
            "Hoofdactiviteiten verenigingsloket werden gewijzigd.");

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdToegevoegd,
            document,
            $"'{contactgegevenWerdToegevoegd.Data.Type} {contactgegevenWerdToegevoegd.Data.Waarde}' werd toegevoegd."
        );

        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdVerwijderd,
            document,
            $"'{contactgegevenWerdVerwijderd.Data.Type} {contactgegevenWerdVerwijderd.Data.Waarde}' werd verwijderd."
        );

        document.Metadata = new Metadata(contactgegevenWerdVerwijderd.Sequence, contactgegevenWerdVerwijderd.Version);
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdGewijzigd,
            document,
            $"'{contactgegevenWerdGewijzigd.Data.Type} {contactgegevenWerdGewijzigd.Data.Waarde}' werd gewijzigd."
        );

        document.Metadata = new Metadata(contactgegevenWerdGewijzigd.Sequence, contactgegevenWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdToegevoegd,
            VertegenwoordigerWerdToegevoegdData.Create(vertegenwoordigerWerdToegevoegd.Data),
            document,
            $"'{vertegenwoordigerWerdToegevoegd.Data.Voornaam} {vertegenwoordigerWerdToegevoegd.Data.Achternaam}' werd toegevoegd als vertegenwoordiger."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdToegevoegd.Sequence, vertegenwoordigerWerdToegevoegd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdGewijzigd,
            document,
            $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam}' werd gewijzigd."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdGewijzigd.Sequence, vertegenwoordigerWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdVerwijderd,
            VertegenwoordigerWerdVerwijderdData.Create(vertegenwoordigerWerdVerwijderd.Data),
            document,
            $"Vertegenwoordiger '{vertegenwoordigerWerdVerwijderd.Data.Voornaam} {vertegenwoordigerWerdVerwijderd.Data.Achternaam}' werd verwijderd."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdVerwijderd.Sequence, vertegenwoordigerWerdVerwijderd.Version);
    }

    public static void Apply(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekeDatastroom, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdUitgeschrevenUitPubliekeDatastroom,
            verenigingWerdUitgeschrevenUitPubliekeDatastroom.Data,
            document,
            $"Vereniging werd uitgeschreven uit de publieke datastroom."
        );

        document.Metadata = new Metadata(verenigingWerdUitgeschrevenUitPubliekeDatastroom.Sequence, verenigingWerdUitgeschrevenUitPubliekeDatastroom.Version);
    }

    public static void Apply(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdIngeschrevenInPubliekeDatastroom, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            verenigingWerdIngeschrevenInPubliekeDatastroom,
            verenigingWerdIngeschrevenInPubliekeDatastroom.Data,
            document,
            $"Vereniging werd ingeschreven in de publieke datastroom."
        );

        document.Metadata = new Metadata(verenigingWerdIngeschrevenInPubliekeDatastroom.Sequence, verenigingWerdIngeschrevenInPubliekeDatastroom.Version);
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

        document.Metadata = new Metadata(locatieWerdToegevoegd.Sequence, locatieWerdToegevoegd.Version);
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

        document.Metadata = new Metadata(locatieWerdGewijzigd.Sequence, locatieWerdGewijzigd.Version);
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

        document.Metadata = new Metadata(locatieWerdVerwijderd.Sequence, locatieWerdVerwijderd.Version);
    }

    private static void AddHistoriekEntry(IEvent @event, BeheerVerenigingHistoriekDocument document, string beschrijving)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                @event.Data,
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    private static void AddHistoriekEntry(IEvent @event,object data, BeheerVerenigingHistoriekDocument document, string beschrijving)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                data,
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }



}
