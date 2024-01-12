namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

public static class ConfigureKestrel
{
    public static void AddEndpoint(this KestrelServerOptions source, IPAddress ipAddress, int port)
    {
        source.Listen(
            new IPEndPoint(ipAddress, port),
            configure: listenOptions =>
            {
                listenOptions.UseConnectionLogging();

                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
    }
}
