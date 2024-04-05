namespace AssociationRegistry.Test.Admin.Api.Grar.When_Address_Match.Fixtures;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class WithNoExactMatchFixture : IAsyncLifetime
{
    private readonly GrarClient _client;
    public string Straatnaam = "Leopold II-laan";
    public string Huisnummer = "99";
    public string Gemeentenaam = "Dendermonde";

    public WithNoExactMatchFixture()
    {
        _client = new GrarClient(new GrarOptionsSection()
        {
            BaseUrl = "http://localhost:8080",
            Timeout = 30,
        }, NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async Task InitializeAsync()
    {
        Result = await _client.GetAddress(Straatnaam, Huisnummer, null, null, Gemeentenaam);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
