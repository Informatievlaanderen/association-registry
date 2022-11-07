namespace AssociationRegistry.Public.Api.Projections;

using Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class VerenigingDetailProjection : SingleStreamAggregation<VerenigingDetailDocument>
{
    public VerenigingDetailDocument Create(VerenigingWerdGeregistreerd verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.VCode,
            Naam = verenigingWerdGeregistreerd.Naam,
        };
}

public class VerenigingDetailDocument
{
    [Identity]
    public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
}
