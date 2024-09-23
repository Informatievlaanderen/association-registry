namespace AssociationRegistry.Test.Public.Api.Controller;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Moq;
using Xunit;

public class PubliekGetAllTests
{
    [Fact]
    public async Task TestController()
    {
        var cancellationToken = CancellationToken.None;
        var mock = new Mock<IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>>();
        var sut = new DetailVerenigingenController(
            new AppSettings());

        await sut.GetAll(mock.Object, Mock.Of<IResponseWriter>(), cancellationToken);

        mock.Verify(x => x.ExecuteAsync(cancellationToken), Times.Once);
    }
}
