namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Common.Clients;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.Extensions.DependencyInjection;

public static class AuthenticationSetup
{
    public const string Initiator = "OVO000001";
    public static IAlbaHost EnsureEachCallIsAuthenticated(this IAlbaHost source, string initiator = Initiator)
    {
        var adminApiClient = new Clients(source.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                         createClientFunc: () => new HttpClient())
           .Authenticated;

        source.BeforeEach(context =>
        {
            context.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
            context.Request.Headers["vr-initiator"] = initiator;

            context.Request.Headers["Authorization"] =
                adminApiClient.HttpClient.DefaultRequestHeaders.GetValues("Authorization").First();
        });

        return source;
    }
}
