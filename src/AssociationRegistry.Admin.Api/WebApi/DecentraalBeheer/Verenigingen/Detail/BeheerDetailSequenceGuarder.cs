namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail;

using AssociationRegistry.Admin.ProjectionHost.Projections;
using AssociationRegistry.Admin.Schema.Detail;
using JasperFx.Events.Projections;
using Marten;
using ProjectionHost.Projections.Detail;
using SequenceGuarding;

public class BeheerDetailSequenceGuarder : SequenceGuarder<BeheerVerenigingDetailDocument>
{
    public BeheerDetailSequenceGuarder(IDocumentStore documentStore)
        : base(documentStore, ValidationMessages.Status412Detail, BeheerVerenigingDetailProjection.ShardName) { }
}
