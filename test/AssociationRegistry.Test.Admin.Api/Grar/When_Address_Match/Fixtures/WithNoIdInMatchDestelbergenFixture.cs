﻿namespace AssociationRegistry.Test.Admin.Api.Grar.When_Address_Match.Fixtures;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using Microsoft.Extensions.Logging.Abstractions;
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
        _client = new GrarClient(new GrarHttpClient(new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080/")
        }), NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async Task InitializeAsync()
    {
        Result = await _client.GetAddressMatches(Straatnaam, Huisnummer, null, Postcode, Gemeentenaam, CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
