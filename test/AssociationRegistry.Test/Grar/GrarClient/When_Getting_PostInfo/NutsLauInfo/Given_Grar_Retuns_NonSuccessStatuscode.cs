namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.NutsLauInfo;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Exceptions;
using FluentAssertions;
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
        var postcode = "9000";

        var httpStatusCode = HttpStatusCode.BadRequest;

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

        var sut = new GrarClient(httpClient.Object, new GrarOptions.GrarClientOptions([1,1,1]), Mock.Of<ILogger<GrarClient>>());

        var exception = await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(async () => await sut.GetPostalNutsLauInformation(postcode, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister);
    }

    [Fact]
    public async ValueTask With_InternalServerError_Then_Throws_Exception()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var postcode = "9000";

        var httpStatusCode = HttpStatusCode.InternalServerError;

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

        var sut = new GrarClient(httpClient.Object, new GrarOptions.GrarClientOptions([1,1,1]), Mock.Of<ILogger<GrarClient>>());

        var exception = await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(async () => await sut.GetPostalNutsLauInformation(postcode, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.AdresKonNietOvergenomenWorden);
    }
}
