namespace AssociationRegistry.Test.Admin.Api.Grar;

using AssociationRegistry.Grar;
using Microsoft.Extensions.Logging.Abstractions;

public class WireMockGrarClient : GrarClient
{
    public WireMockGrarClient()
        : base(new GrarHttpClient(new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080")
        }), NullLogger<GrarClient>.Instance)
    {
    }
}
