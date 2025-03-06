namespace AssociationRegistry.Test.GrarClient.Fixtures;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using Grar.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        }), NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Result = await _client.GetAddressMatches(Straatnaam, Huisnummer, busnummer: null, Postcode, Gemeentenaam, CancellationToken.None);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
