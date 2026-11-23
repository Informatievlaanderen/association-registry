namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen;

using AssociationRegistry.Admin.Schema.Relatieaanvragen;
using Marten;
using ProjectionHost.Projections.Relatieaanvragen;
using Verenigingen.SequenceGuarding;

public class RelatieaanvraagSequenceGuarder : SequenceGuarder<RelatieaanvraagDocument>
{
    public RelatieaanvraagSequenceGuarder(IDocumentStore documentStore)
        : base(documentStore, ValidationMessages.Status412Relatieaanvragen, RelatieaanvraagProjection.ShardName) { }
}
