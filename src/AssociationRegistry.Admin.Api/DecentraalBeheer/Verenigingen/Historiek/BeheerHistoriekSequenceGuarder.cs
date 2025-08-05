namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using AssociationRegistry.Admin.Schema.Historiek;
using Detail.SequenceGuarding;
using JasperFx.Events.Projections;
using Marten;

public class BeheerHistoriekSequenceGuarder : SequenceGuarder<BeheerVerenigingHistoriekDocument>
{
    public BeheerHistoriekSequenceGuarder(IDocumentStore documentStore) :
        base(
            documentStore,
            ValidationMessages.Status412Historiek,
            new ShardName("AssociationRegistry.Admin.ProjectionHost.Projections.Historiek.BeheerVerenigingHistoriekProjection"))
    { }
}
