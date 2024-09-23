namespace AssociationRegistry.Test.Admin.Api.Controller;

using Moq;
using Public.Api.Infrastructure.ConfigurationBindings;
using Public.Api.Queries;
using Public.Api.Verenigingen.Detail;
using Public.Schema.Detail;
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
