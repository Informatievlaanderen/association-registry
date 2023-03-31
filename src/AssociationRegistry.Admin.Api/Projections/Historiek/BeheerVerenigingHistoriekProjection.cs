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
            $"Vereniging werd geregistreerd met naam '{verenigingWerdGeregistreerd.Data.Naam}'.",
            new VerenigingWerdgeregistreerdData(verenigingWerdGeregistreerd.Data));

        return beheerVerenigingHistoriekDocument;
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            document,
            $"Naam werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}'.",
            new NaamWerdGewijzigdData(naamWerdGewijzigd.Data.Naam));

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            document,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.",
            new KorteNaamWerdGewijzigdData(korteNaamWerdGewijzigd.Data.KorteNaam));

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            document,
            $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.",
            new KorteBeschrijvingWerdGewijzigdData(korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving));

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        if (startdatumWerdGewijzigd.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd gewijzigd naar '{startDatumString}'.",
                new StartdatumWerdGewijzigdData(startDatumString)
            );
        }
        else
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd verwijderd.",
                new StartdatumWerdGewijzigdData(null)
            );
    }

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdToegevoegd,
            document,
            $"{contactgegevenWerdToegevoegd.Data.Type} contactgegeven werd toegevoegd."
        );

        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    private static void AddHistoriekEntry(IEvent @event, BeheerVerenigingHistoriekDocument document, string beschrijving, IHistoriekData data)
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
