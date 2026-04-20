namespace AssociationRegistry.Integrations.Wegwijs.Clients;

using Responses;

public interface IWegwijsClient
{
    Task<OrganisationResponse> GetOrganisationByOvoCode(string ovoCode, CancellationToken cancellationToken = default);
}
