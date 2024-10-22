namespace AssociationRegistry.Test.Public.Api.Controller;

using AssociationRegistry.Public.Schema.Detail;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class PubliekGetAllTests : IClassFixture<PubliekGetAllFixture>
{
    private readonly PubliekGetAllFixture _fixture;

    public PubliekGetAllTests(PubliekGetAllFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void VerifyQueryIsCalled()
        => _fixture.Query.Verify(x => x.ExecuteAsync(CancellationToken.None), Times.Once);

    [Fact]
    public void VerifyWriterIsCalled()
        => _fixture.StreamWriter.Verify(x => x.WriteAsync(It.IsAny<IAsyncEnumerable<PubliekVerenigingDetailDocument>>(),
                                                          It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public void VerifyS3ClientPutIsCalled()
        => _fixture.S3Client.Verify(x => x.PutAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public void VerifyS3ClientGetPreSignedUrlIsCalled()
        => _fixture.S3Client.Verify(x => x.GetPreSignedUrlAsync(It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public void VerifyRedirectWithLocationHeaderIsProvided()
    {
        var redirectResult = Assert.IsType<RedirectResult>(_fixture.Response);
        redirectResult.Url.Should().Be(_fixture.RedirectUrl);
    }
}
