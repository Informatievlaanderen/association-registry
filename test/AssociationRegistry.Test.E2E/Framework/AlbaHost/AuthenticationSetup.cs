namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Common.Fixtures;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;

public static class AuthenticationSetup
{
    public const string Initiator = "OVO000001";

    public static IAlbaHost EnsureEachCallIsAuthenticated(this IAlbaHost source, HttpClient httpClient, string initiator = Initiator)
    {
        source.BeforeEach(context =>
        {
            context.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
            context.Request.Headers["vr-initiator"] = initiator;

            context.Request.Headers["Authorization"] =
                httpClient.DefaultRequestHeaders.GetValues("Authorization").First();
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
