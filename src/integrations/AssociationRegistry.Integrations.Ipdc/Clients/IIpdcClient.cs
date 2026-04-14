namespace AssociationRegistry.Integrations.Ipdc.Clients;

using Responses;

public interface IIpdcClient
{
    Task<IpdcProductResponse?> GetById(string ipdcProductNummer, CancellationToken cancellationToken = default);
}
