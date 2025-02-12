namespace AssociationRegistry.Test.Common.Fixtures;

using Admin.Api.Constants;
using Common.Clients;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using System.Net.Http.Headers;

public class AdminApiClients : IDisposable
{
    private readonly Func<HttpClient> _createClientFunc;
    private readonly OAuth2IntrospectionOptions _oAuth2IntrospectionOptions;

    public AdminApiClients(OAuth2IntrospectionOptions oAuth2IntrospectionOptions, Func<HttpClient> createClientFunc)
    {
        _oAuth2IntrospectionOptions = oAuth2IntrospectionOptions;
        _createClientFunc = createClientFunc;
    }

    public HttpClient GetAuthenticatedHttpClient()
        => CreateMachine2MachineClientFor(clientId: "vloketClient", Security.Scopes.Admin, clientSecret: "secret").GetAwaiter().GetResult();

    private HttpClient GetSuperAdminHttpClient()
        => CreateMachine2MachineClientFor(clientId: "superAdminClient", Security.Scopes.Admin, clientSecret: "secret").GetAwaiter()
           .GetResult();

    public AdminApiClient Authenticated
        => new(GetAuthenticatedHttpClient());

    public AdminApiClient SuperAdmin
        => new(GetSuperAdminHttpClient());

    public AdminApiClient Unauthenticated
        => new(_createClientFunc());

    public AdminApiClient Unauthorized
        => new(CreateMachine2MachineClientFor(clientId: "vloketClient", scope: "vo_info", clientSecret: "secret").GetAwaiter().GetResult());

    public void Dispose()
    {
    }

    private async Task<HttpClient> CreateMachine2MachineClientFor(
        string clientId,
        string scope,
        string clientSecret)
    {
        var tokenClient = new TokenClient(
            client: () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{_oAuth2IntrospectionOptions.Authority}/connect/token",
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
        var httpClientFor = _createClientFunc();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", token);

        return httpClientFor;
    }
}
