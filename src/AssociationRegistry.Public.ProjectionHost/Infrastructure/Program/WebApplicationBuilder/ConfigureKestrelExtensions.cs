namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

public static class ConfigureKestrel
{
    public static void AddEndpoint(this KestrelServerOptions source, IPAddress ipAddress, int port)
    {
        source.Listen(
            new IPEndPoint(ipAddress, port),
            listenOptions =>
            {
                listenOptions.UseConnectionLogging();

                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
    }
}
