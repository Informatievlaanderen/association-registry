namespace AssociationRegistry.Test.Admin.Api.Grar.When_Heradresseren_Locaties.Fixtures;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
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
        Result = await _client.GetAddressById("200001", CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
