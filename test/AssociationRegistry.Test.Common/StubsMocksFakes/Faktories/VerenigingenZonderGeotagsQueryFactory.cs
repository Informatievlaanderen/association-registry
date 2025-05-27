namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Admin.Api.Queries;
using global::AutoFixture;
using Moq;

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

    public Mock<IVerenigingenWithoutGeotagsQuery> ReturnsRandomGeotags()
        => Returns(_fixture.CreateMany<string>().ToArray());
}
