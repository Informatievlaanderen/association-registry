namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using AssociationRegistry.Admin.Schema.Detail;
using JasperFx.Events.Projections;
using Marten;
using SequenceGuarding;

public class BeheerDetailSequenceGuarder : SequenceGuarder<BeheerVerenigingDetailDocument>
{
    public BeheerDetailSequenceGuarder(IDocumentStore documentStore) :
        base(
            documentStore,
            ValidationMessages.Status412Detail,
            new ShardName("AssociationRegistry.Admin.ProjectionHost.Projections.Detail.BeheerVerenigingDetailProjection"))
    { }
}
