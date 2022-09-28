namespace AssociationRegistry.Test.Acm.Api.IntegrationTests.Fixtures;

using System.Net.Http.Headers;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

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
        => await CreateMachine2MachineClientFor("acmClient", scope, "secret");

    private async Task<HttpClient> CreateMachine2MachineClientFor(string clientId, string scope, string clientSecret)
    {
        var editApiConfiguration = _fixture.ConfigurationRoot!.GetSection(nameof(OAuth2IntrospectionOptions))
            .Get<OAuth2IntrospectionOptions>();

        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{editApiConfiguration.Authority}/connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);
        var token = acmResponse.AccessToken;
        var httpClientFor = _fixture.Server.CreateClient();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return httpClientFor;
    }
}
