namespace AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; set; }
}
