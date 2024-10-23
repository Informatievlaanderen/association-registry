namespace AssociationRegistry.Test.Public.Api.DetailAll;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Public.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class PubliekGetAllTests
{
    private readonly Fixture _fixture;
    private readonly IActionResult _response;
    private readonly string _redirectUrl;
    private readonly MemoryStream _stream;
    private readonly Mock<IDetailAllS3Client> _s3ClientMock;

    public PubliekGetAllTests()
    {
        _fixture = new Fixture().CustomizePublicApi();
        var sut = new DetailVerenigingenController(new AppSettings());
        _redirectUrl = "https://localhost:4566";
        _stream = new MemoryStream();

        var queryMock = new Mock<IPubliekVerenigingenDetailAllQuery>();
        var streamWriterMock = new Mock<IDetailAllStreamWriter>();
        _s3ClientMock = new Mock<IDetailAllS3Client>();

        SetupMocks(_s3ClientMock, queryMock, streamWriterMock);

        _response = sut.GetAll(queryMock.Object, streamWriterMock.Object, _s3ClientMock.Object, CancellationToken.None)
                       .GetAwaiter()
                       .GetResult();
    }

    [Fact]
    public void VerifyS3ClientPutIsCalled()
        => _s3ClientMock.Verify(x => x.PutAsync(_stream, It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public void VerifyRedirectWithLocationHeaderIsProvided()
    {
        var redirectResult = Assert.IsType<RedirectResult>(_response);
        redirectResult.Url.Should().Be(_redirectUrl);
    }

    private async IAsyncEnumerable<PubliekVerenigingDetailDocument> GetData()
    {
        var docs = _fixture.CreateMany<PubliekVerenigingDetailDocument>();

        foreach (var doc in docs)
        {
            yield return doc;
        }
    }

    private void SetupMocks(
        Mock<IDetailAllS3Client> s3ClientMock,
        Mock<IPubliekVerenigingenDetailAllQuery> queryMock,
        Mock<IDetailAllStreamWriter> streamWriterMock)
    {
        var data = GetData();

        queryMock.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(data);

        streamWriterMock.Setup(s => s.WriteAsync(data, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_stream);

        s3ClientMock.Setup(x => x.GetPreSignedUrlAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_redirectUrl);
    }
}
