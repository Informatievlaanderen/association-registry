namespace AssociationRegistry.Test.Public.Api.Controller;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

public class PubliekGetAllTests
{
    [Fact]
    public async Task VerifyQueryIsCalled()
    {
        var cancellationToken = CancellationToken.None;
        var mock = new Mock<IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>>();

        var sut = new DetailVerenigingenController(
            new AppSettings());

        await sut.GetAll(mock.Object, Mock.Of<IResponseWriter>(), cancellationToken);

        mock.Verify(x => x.ExecuteAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task VerifyWriterIsCalled()
    {
        var cancellationToken = CancellationToken.None;
        var mock = new Mock<IResponseWriter>();

        var sut = new DetailVerenigingenController(
            new AppSettings());

        await sut.GetAll(new Mock<IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>>().Object, mock.Object, cancellationToken);

        mock.Verify(
            x => x.Write(It.IsAny<HttpResponse>(), It.IsAny<IAsyncEnumerable<PubliekVerenigingDetailDocument>>(),
                         It.IsAny<CancellationToken>()), Times.Once);
    }
}
