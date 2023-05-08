namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using System.Linq;
using Constants;
using Detail;
using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

        AddHistoriekEntry(
            verenigingWerdGeregistreerd,
            beheerVerenigingHistoriekDocument,
            $"Vereniging werd geregistreerd met naam '{verenigingWerdGeregistreerd.Data.Naam}'.");

        return beheerVerenigingHistoriekDocument;
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            document,
            $"Naam werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}'.");

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            document,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.");

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            document,
            $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.");

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
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
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd verwijderd."
            );
    }

    public void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
            document,
            "Hoofdactiviteiten verenigingsloket werden gewijzigd.");

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdToegevoegd,
            document,
            $"{contactgegevenWerdToegevoegd.Data.Type} {contactgegevenWerdToegevoegd.Data.Waarde} werd toegevoegd."
        );

        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    public void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdVerwijderd,
            document,
            $"{contactgegevenWerdVerwijderd.Data.Type} {contactgegevenWerdVerwijderd.Data.Waarde} werd verwijderd."
        );

        document.Metadata = new Metadata(contactgegevenWerdVerwijderd.Sequence, contactgegevenWerdVerwijderd.Version);
    }

    public void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdGewijzigd,
            document,
            $"{contactgegevenWerdGewijzigd.Data.Type} {contactgegevenWerdGewijzigd.Data.Waarde} werd gewijzigd."
        );

        document.Metadata = new Metadata(contactgegevenWerdGewijzigd.Sequence, contactgegevenWerdGewijzigd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdToegevoegd,
            document,
            $"{vertegenwoordigerWerdToegevoegd.Data.Voornaam} " +
            $"{vertegenwoordigerWerdToegevoegd.Data.Achternaam} werd toegevoegd als vertegenwoordiger."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdToegevoegd.Sequence, vertegenwoordigerWerdToegevoegd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdGewijzigd,
            document,
            $"Vertegenwoordiger {vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam} " +
            $"met ID {vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId} " +
            $"werd gewijzigd."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdGewijzigd.Sequence, vertegenwoordigerWerdGewijzigd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            vertegenwoordigerWerdVerwijderd,
            document,
            $"Vertegenwoordiger {vertegenwoordigerWerdVerwijderd.Data.Voornaam} {vertegenwoordigerWerdVerwijderd.Data.Achternaam} werd verwijderd."
        );

        document.Metadata = new Metadata(vertegenwoordigerWerdVerwijderd.Sequence, vertegenwoordigerWerdVerwijderd.Version);
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
}
