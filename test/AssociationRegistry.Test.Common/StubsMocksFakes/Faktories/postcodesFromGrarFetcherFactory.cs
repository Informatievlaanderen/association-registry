namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Grar.NutsLau;
using global::AutoFixture;
using Moq;

public class postcodesFromGrarFetcherFactory
{
    public Mock<IPostcodesFromGrarFetcher> Mock()
        => new();

    public Mock<IPostcodesFromGrarFetcher> ReturnsPostcodes(string[] returns)
    {
        var mock = new Mock<IPostcodesFromGrarFetcher>();
        mock.Setup(x => x.FetchPostalCodes(It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns.ToArray);
        return mock;
    }

    public Mock<IPostcodesFromGrarFetcher> Throws(Exception? exception = null)
    {
        var mock = new Mock<IPostcodesFromGrarFetcher>();
        mock.Setup(x => x.FetchPostalCodes(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception ?? new Exception("Mocked exception"));
        return mock;
    }


    public postcodesFromGrarFetcherFactory(IFixture fixture)
    {
    }
}
