namespace AssociationRegistry.Test.Locaties.When_Heradresseren_Locaties.Fixtures;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.Framework;
using Xunit;

public class DetailFixture : IAsyncLifetime
{
    private readonly GrarClient _client;

    public DetailFixture()
    {
        _client = new WireMockGrarClient();
    }

    public AddressDetailResponse Result { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Result = await _client.GetAddressById(adresId: "200001", CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
