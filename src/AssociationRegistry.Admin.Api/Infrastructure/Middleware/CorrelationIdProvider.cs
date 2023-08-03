namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

class CorrelationIdProvider : ICorrelationIdProvider
{
    public string CorrelationId { get; set; }
}
