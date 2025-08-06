namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek;

using AssociationRegistry.Admin.Schema.Historiek;
using JasperFx.Events.Projections;
using Marten;
using SequenceGuarding;

public class BeheerHistoriekSequenceGuarder : SequenceGuarder<BeheerVerenigingHistoriekDocument>
{
    public BeheerHistoriekSequenceGuarder(IDocumentStore documentStore) :
        base(
            documentStore,
            ValidationMessages.Status412Historiek,
            new ShardName("AssociationRegistry.Admin.ProjectionHost.Projections.Historiek.BeheerVerenigingHistoriekProjection"))
    { }
}
