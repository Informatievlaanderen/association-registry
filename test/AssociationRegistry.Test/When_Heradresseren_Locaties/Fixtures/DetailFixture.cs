namespace AssociationRegistry.Test.When_Heradresseren_Locaties.Fixtures;

using Common.Framework;
using Grar;
using Grar.Models;
using Xunit;

public class DetailFixture : IAsyncLifetime
{
    private readonly GrarClient _client;

    public DetailFixture()
    {
        _client = new WireMockGrarClient();
    }

    public AddressDetailResponse Result { get; private set; }

    public async Task InitializeAsync()
    {
        Result = await _client.GetAddressById(adresId: "200001", CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
