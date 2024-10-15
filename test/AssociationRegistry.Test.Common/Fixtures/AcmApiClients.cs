namespace AssociationRegistry.Test.Common.Fixtures;

using Acm.Api.Constants;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using JasperFx.Core;
using System.Net.Http.Headers;

public class AcmApiClients : IDisposable
{
    private readonly Func<HttpClient> _createClientFunc;
    private readonly OAuth2IntrospectionOptions _oAuth2IntrospectionOptions;

    public AcmApiClients(OAuth2IntrospectionOptions oAuth2IntrospectionOptions, Func<HttpClient> createClientFunc)
    {
        _oAuth2IntrospectionOptions = oAuth2IntrospectionOptions;
        _createClientFunc = createClientFunc;

        Authenticated = new AcmApiClient(CreateMachine2MachineClientFor(clientId: "acmClient", Security.Scopes.ACM, clientSecret: "secret")
                                        .GetAwaiter().GetResult());

        Unauthenticated = new AcmApiClient(_createClientFunc());

        Unauthorized = new AcmApiClient(CreateMachine2MachineClientFor(clientId: "acmClient", Security.Scopes.Info, clientSecret: "secret")
                                       .GetAwaiter().GetResult());
    }

    public AcmApiClient Authenticated { get; }
    public AcmApiClient Unauthenticated { get; }
    public AcmApiClient Unauthorized { get; }

    public void Dispose()
    {
        Authenticated.SafeDispose();
        Unauthenticated.SafeDispose();
        Unauthorized.SafeDispose();
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
