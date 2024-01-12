namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

internal class CorrelationIdProvider : ICorrelationIdProvider
{
    public string CorrelationId { get; set; }
}
