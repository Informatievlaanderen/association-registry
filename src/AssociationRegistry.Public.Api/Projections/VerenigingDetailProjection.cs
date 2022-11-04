namespace AssociationRegistry.Public.Api.Projections;

using Events;
using Marten.Events.Projections;
using Marten.Schema;

public class VerenigingDetailProjection : MultiStreamAggregation<VerenigingDetailDocument, string>
{
    public VerenigingDetailProjection()
    {
        Identity<VerenigingWerdGeregistreerd>(verenigingWerdGeregistreerd => verenigingWerdGeregistreerd.VCode);
    }

    public void Apply(VerenigingWerdGeregistreerd verenigingWerdGeregistreerd, VerenigingDetailDocument document)
    {
        document.VCode = verenigingWerdGeregistreerd.VCode;
        document.Naam = verenigingWerdGeregistreerd.Naam;
    }
}

public class VerenigingDetailDocument
{
    [Identity]
    public string VCode { get; set; }
    public string Naam { get; set; }
}
