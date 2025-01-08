namespace AssociationRegistry.Test.Common.Framework;

using Grar;
using Grar.Clients;
using Microsoft.Extensions.Logging.Abstractions;

public class WireMockGrarClient : GrarClient
{
    public WireMockGrarClient()
        : base(new GrarHttpClient(new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8080"),
        }), NullLogger<GrarClient>.Instance)
    {
    }
}
