namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

class CorrelationIdProvider : ICorrelationIdProvider
{
    public CorrelationIdProvider()
    {

    }

    public string CorrelationId { get; set; }
}
