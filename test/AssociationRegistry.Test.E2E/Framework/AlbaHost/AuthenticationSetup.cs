namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Common.Clients;
using Common.Fixtures;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.Extensions.DependencyInjection;

public static class AuthenticationSetup
{
    public const string Initiator = "OVO000001";

    public static IAlbaHost EnsureEachCallIsAuthenticated(this IAlbaHost source, string initiator = Initiator)
    {
        var clients = new Clients(source.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                  createClientFunc: () => new HttpClient());

        var adminApiClient = clients.Authenticated;

        source.BeforeEach(context =>
        {
            context.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
            context.Request.Headers["vr-initiator"] = initiator;

            context.Request.Headers["Authorization"] =
                adminApiClient.HttpClient.DefaultRequestHeaders.GetValues("Authorization").First();
        });

        return source;
    }

    public static IAlbaHost EnsureEachCallIsAuthenticatedForAcmApi(this IAlbaHost source)
    {
        var clients = new AcmApiClients(source.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                                          createClientFunc: () => new HttpClient());

        var acmApiClient = clients.Authenticated;

        source.BeforeEach(context =>
        {
            context.Request.Headers["Authorization"] =
                acmApiClient.HttpClient.DefaultRequestHeaders.GetValues("Authorization").First();
        });

        return source;
    }
}
