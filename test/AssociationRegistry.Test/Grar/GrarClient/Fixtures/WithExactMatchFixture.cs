namespace AssociationRegistry.Test.Grar.GrarClient.Fixtures;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class WithExactMatchFixture : IAsyncLifetime
{
    private readonly GrarClient _client;
    public string Straatnaam = "Leopold II-laan";
    public string Huisnummer = "99";
    public string Postcode = "9200";
    public string Gemeentenaam = "Dendermonde";

    public WithExactMatchFixture()
    {
        _client = new GrarClient(new GrarHttpClient(new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8080"),
        }), new GrarOptions.GrarClientOptions([1,1,1]), NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async Task InitializeAsync()
    {
        Result = await _client.GetAddressMatches(Straatnaam, Huisnummer, busnummer: null, Postcode, Gemeentenaam, CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
