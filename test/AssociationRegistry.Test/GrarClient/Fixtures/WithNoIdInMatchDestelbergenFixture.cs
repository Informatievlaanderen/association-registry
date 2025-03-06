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

public class WithNoIdInMatchDestelbergenFixture : IAsyncLifetime
{
    private readonly GrarClient _client;
    public string Straatnaam = "Dendermondsesteenweg";
    public string Huisnummer = "32747";
    public string Gemeentenaam = "Destelbergen";
    public string Postcode = "9070";

    public WithNoIdInMatchDestelbergenFixture()
    {
        _client = new GrarClient(new GrarHttpClient(new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8080/"),
        }), NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Result = await _client.GetAddressMatches(Straatnaam, Huisnummer, busnummer: null, Postcode, Gemeentenaam, CancellationToken.None);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
