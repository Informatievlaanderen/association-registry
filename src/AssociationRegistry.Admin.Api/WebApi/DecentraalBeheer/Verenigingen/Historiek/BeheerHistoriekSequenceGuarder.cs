namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek;

using AssociationRegistry.Admin.ProjectionHost.Projections;
using AssociationRegistry.Admin.Schema.Historiek;
using JasperFx.Events.Projections;
using Marten;
using ProjectionHost.Projections.Historiek;
using SequenceGuarding;

public class BeheerHistoriekSequenceGuarder : SequenceGuarder<BeheerVerenigingHistoriekDocument>
{
    public BeheerHistoriekSequenceGuarder(IDocumentStore documentStore)
        : base(documentStore, ValidationMessages.Status412Historiek, BeheerVerenigingHistoriekProjection.ShardName) { }
}
