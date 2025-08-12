namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.List;

using AssociationRegistry.Grar.Exceptions;
using Events;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.Clients;
using Microsoft.Extensions.Logging;
using Moq;
using Resources;
using System.Net;
using Xunit;

public class Given_Grar_Retuns_NonSuccessStatuscode
{
    [Fact]
    public async ValueTask With_BadRequest_Then_Throws_Exception()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var offset = "100";
        var limit = "200";

        var httpStatusCode = HttpStatusCode.BadRequest;

        httpClient.Setup(x => x.GetPostInfoList(offset, limit, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

        var sut = new GrarClient(httpClient.Object, new GrarOptions.GrarClientOptions([1,1,1]), Mock.Of<ILogger <GrarClient>>());

        var exception = await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(async () => await sut.GetPostalInformationList(offset, limit, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister);
    }

    [Fact]
    public async ValueTask With_InternalServerError_Then_Throws_Exception()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var offset = "100";
        var limit = "200";

        var httpStatusCode = HttpStatusCode.InternalServerError;

        httpClient.Setup(x => x.GetPostInfoList(offset, limit, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

        var sut = new GrarClient(httpClient.Object, new GrarOptions.GrarClientOptions([1,1,1]), Mock.Of<ILogger <GrarClient>>());

        var exception = await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(async () => await sut.GetPostalInformationList(offset, limit, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.AdresKonNietOvergenomenWorden);
    }
}
