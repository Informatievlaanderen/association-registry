namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class AcmIntegrationTestHelper
{
    private readonly VerenigingAcmApiFixture _fixture;

    public AcmIntegrationTestHelper(VerenigingAcmApiFixture fixture)
    {
        _fixture = fixture;
    }

    public HttpClient HttpClient
        => _fixture.Server.CreateClient();

    public async Task<HttpClient> CreateAcmClient()
        => await CreateAcmClient("dv_verenigingsregister_hoofdvertegenwoordigers");

    public async Task<HttpClient> CreateAcmClient(string scope)
        => await CreateMachine2MachineClientFor(clientId: "acmClient", scope, clientSecret: "secret");

    private async Task<HttpClient> CreateMachine2MachineClientFor(
        string clientId,
        string scope,
        string clientSecret)
    {
        var editApiConfiguration = _fixture.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                                           .Get<OAuth2IntrospectionOptions>();

        var tokenClient = new TokenClient(
            client: () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{editApiConfiguration.Authority}/connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>(key: "scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        var token = acmResponse.AccessToken;
        var httpClientFor = _fixture.Server.CreateClient();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", token);

        return httpClientFor;
    }
}
