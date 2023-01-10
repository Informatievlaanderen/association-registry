namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using Framework;
using Vereniging;
using Detail;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new(
            verenigingWerdGeregistreerd.Data.VCode,
            new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    nameof(VerenigingWerdGeregistreerd),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Initiator),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                ),
            },
            new Metadata(verenigingWerdGeregistreerd.Sequence));
}

public record BeheerVerenigingHistoriekDocument(
    [property: Identity] string VCode,
    List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen,
    Metadata Metadata) : IMetadata;

public record BeheerVerenigingHistoriekGebeurtenis(
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
