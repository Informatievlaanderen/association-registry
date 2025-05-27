namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Grar.NutsLau;
using global::AutoFixture;
using Moq;

public class NutsLauFromGrarFetcherFactory
{
    private readonly IFixture _fixture;

    public NutsLauFromGrarFetcherFactory(IFixture fixture)
    {
        _fixture = fixture;
    }

    public Mock<INutsLauFromGrarFetcher> Mock()
        => new();

    public Mock<INutsLauFromGrarFetcher> Returns(PostalNutsLauInfo[] returns)
    {
        var mock = new Mock<INutsLauFromGrarFetcher>();
        mock.Setup(x => x.GetFlemishAndBrusselsNutsAndLauByPostcode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns.ToArray);
        return mock;
    }

    public Mock<INutsLauFromGrarFetcher> ReturnsRandomPostalNutsLauInfos()
    {
        var mock = new Mock<INutsLauFromGrarFetcher>();
        mock.Setup(x => x.GetFlemishAndBrusselsNutsAndLauByPostcode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.CreateMany<PostalNutsLauInfo>().ToArray);
        return mock;
    }
}
