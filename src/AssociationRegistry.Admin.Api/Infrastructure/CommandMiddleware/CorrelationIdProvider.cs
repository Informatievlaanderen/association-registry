namespace AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;

internal class CorrelationIdProvider : ICorrelationIdProvider
{
    public string CorrelationId { get; set; }
}
