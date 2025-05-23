﻿namespace AssociationRegistry.Test.Grar.GrarClient.Fixtures;

using AssociationRegistry.Grar.Clients;
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
        _client = new GrarClient(new GrarHttpClient(new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8080/"),
        }),new GrarOptions.GrarClientOptions([1,1,1]), NullLogger<GrarClient>.Instance);
    }

    public IReadOnlyCollection<AddressMatchResponse> Result { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Result = await _client.GetAddressMatches(Straatnaam, Huisnummer, busnummer: null, postcode: null, Gemeentenaam,
                                                 CancellationToken.None);
    }

    public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
}
