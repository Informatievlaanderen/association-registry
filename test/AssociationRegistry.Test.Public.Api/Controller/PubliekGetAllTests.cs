namespace AssociationRegistry.Test.Public.Api.Controller;

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
    public void VerifyS3ClientPutIsCalled()
        => _fixture.S3Client.Verify(x => x.PutAsync(_fixture.Stream, It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public void VerifyRedirectWithLocationHeaderIsProvided()
    {
        var redirectResult = Assert.IsType<RedirectResult>(_fixture.Response);
        redirectResult.Url.Should().Be(_fixture.RedirectUrl);
    }
}
