namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Admin.Api.Queries;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;
using Moq;
using Vereniging;

public class VerenigingenZonderGeotagsQueryFactory
{
    private readonly IFixture _fixture;

    public VerenigingenZonderGeotagsQueryFactory(IFixture fixture)
    {
        _fixture = fixture;
    }

    public Mock<IVerenigingenWithoutGeotagsQuery> Returns(IEnumerable<string> returns)
    {
        var mock = new Mock<IVerenigingenWithoutGeotagsQuery>();
        mock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(returns.ToArray);
        return mock;
    }

    public Mock<IVerenigingenWithoutGeotagsQuery> Throws(Exception? exception = null)
    {
        var mock = new Mock<IVerenigingenWithoutGeotagsQuery>();
        mock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception ?? new Exception("Mocked exception"));
        return mock;
    }

    public Mock<IVerenigingenWithoutGeotagsQuery> ReturnsRandomGeotags()
        => Returns(_fixture.CreateMany<VCode>().Select(x => x.ToString()).ToArray());
}
