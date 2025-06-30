namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Grar.NutsLau;
using Events;
using global::AutoFixture;
using Grar.Clients;
using Grar.Models;
using Moq;

public class GrarClientFactory
{
    public Mock<IGrarClient> Mock()
        => new();

    public Mock<IGrarClient> GetAdresByIdReturnsAdres(string adresId, AddressDetailResponse returns)
    {
        var mock = new Mock<IGrarClient>();
        mock.Setup(x => x.GetAddressById(adresId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);
        return mock;
    }

    public GrarClientFactory(IFixture fixture)
    {
    }
}
