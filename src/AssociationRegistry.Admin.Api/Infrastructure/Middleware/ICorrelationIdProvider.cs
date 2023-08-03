namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; set; }
}
