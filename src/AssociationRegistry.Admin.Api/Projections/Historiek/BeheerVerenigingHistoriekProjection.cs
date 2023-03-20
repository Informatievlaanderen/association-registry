namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using Framework;
using Detail;
using Events;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
    {
        var initiator = verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = verenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        return new BeheerVerenigingHistoriekDocument
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    $"Vereniging werd aangemaakt met naam '{verenigingWerdGeregistreerd.Data.Naam}' door {initiator} op datum {tijdstip}",
                    initiator,
                    tijdstip
                ),
            },
            Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence, verenigingWerdGeregistreerd.Version),
        };
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        var initiator = naamWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Naam vereniging werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}' door {initiator} op datum {tijdstip}",
                initiator,
                tijdstip
            )
        );
        document.Metadata = document.Metadata with { Sequence = naamWerdGewijzigd.Sequence, Version = naamWerdGewijzigd.Version };
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                nameof(KorteNaamWerdGewijzigd),
                korteNaamWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator),
                korteNaamWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Tijdstip)
            )
        );
        document.Metadata = document.Metadata with { Sequence = korteNaamWerdGewijzigd.Sequence, Version = korteNaamWerdGewijzigd.Version };
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                nameof(KorteBeschrijvingWerdGewijzigd),
                korteBeschrijvingWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator),
                korteBeschrijvingWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Tijdstip)
            )
        );
        document.Metadata = document.Metadata with { Sequence = korteBeschrijvingWerdGewijzigd.Sequence, Version = korteBeschrijvingWerdGewijzigd.Version };
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                nameof(StartdatumWerdGewijzigd),
                startdatumWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator),
                startdatumWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Tijdstip)
            )
        );
        document.Metadata = document.Metadata with { Sequence = startdatumWerdGewijzigd.Sequence, Version = startdatumWerdGewijzigd.Version };
    }

    public void Apply(IEvent<ContactInfoLijstWerdGewijzigd> contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                nameof(ContactInfoLijstWerdGewijzigd),
                contactInfoLijstWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator),
                contactInfoLijstWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Tijdstip)
            )
        );
        document.Metadata = document.Metadata with { Sequence = contactInfoLijstWerdGewijzigd.Sequence, Version = contactInfoLijstWerdGewijzigd.Version };
    }
}

// TODO bekijken of Metadata weg kan?
public class BeheerVerenigingHistoriekDocument : IMetadata, IVCode
{
    [Identity] public string VCode { get; set; } = null!;
    public List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
}

public record BeheerVerenigingHistoriekGebeurtenis(
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
