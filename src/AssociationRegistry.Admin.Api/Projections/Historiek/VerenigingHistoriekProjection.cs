namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using Framework;
using Vereniging;
using Detail;
using Infrastructure;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class VerenigingHistoriekProjection : SingleStreamAggregation<VerenigingHistoriekDocument>
{
    public VerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new(
            verenigingWerdGeregistreerd.Data.VCode,
            new List<VerenigingHistoriekGebeurtenis>
            {
                new(
                    nameof(VerenigingWerdGeregistreerd),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Initiator),
                    verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                ),
            },
            new Metadata(verenigingWerdGeregistreerd.Sequence));
}

public record VerenigingHistoriekDocument(
    [property: Identity] string VCode,
    List<VerenigingHistoriekGebeurtenis> Gebeurtenissen,
    Metadata Metadata) : IMetadata;

public record VerenigingHistoriekGebeurtenis(
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
