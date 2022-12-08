namespace AssociationRegistry.Admin.Api.Projections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework;
using Vereniging;
using Infrastructure;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Schema;
using IEvent = Marten.Events.IEvent;

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
                )
            });
}

public record VerenigingHistoriekDocument(
    [property: Identity] string VCode,
    List<VerenigingHistoriekGebeurtenis> Gebeurtenissen);

public record VerenigingHistoriekGebeurtenis(
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
