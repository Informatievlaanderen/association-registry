namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using Framework;
using Detail;
using Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    nameof(VerenigingWerdGeregistreerd),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Initiator),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                ),
            },
            Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence, verenigingWerdGeregistreerd.Version),
        };

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        document.Gebeurtenissen.Add(
            new BeheerVerenigingHistoriekGebeurtenis(
                nameof(NaamWerdGewijzigd),
                naamWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Initiator),
                naamWerdGewijzigd.GetHeaderString(MetadataHeaderNames.Tijdstip)
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
