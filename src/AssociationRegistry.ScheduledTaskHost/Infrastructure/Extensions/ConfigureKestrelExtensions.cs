namespace AssociationRegistry.ScheduledTaskHost.Infrastructure.Extensions;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

internal static class ConfigureKestrel
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
