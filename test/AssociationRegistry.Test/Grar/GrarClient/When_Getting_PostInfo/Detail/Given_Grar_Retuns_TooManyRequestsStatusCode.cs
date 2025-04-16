namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.Detail;

using AssociationRegistry.Grar.Clients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Resources;
using System.Net;
using Xunit;

public class Given_Grar_Retuns_TooManyRequestsStatusCode
{
    [Fact]
    public async Task Then_Throws_Exception()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var postcode = "9000";

        var httpStatusCode = HttpStatusCode.TooManyRequests;

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

        var sut = new GrarClient(httpClient.Object, new GrarOptions.HttpClientOptions()
        {
            BackoffInMs = [1,1,1],
        }, Mock.Of<ILogger<GrarClient>>());

        var exception = await Assert.ThrowsAsync<TooManyRequestException>(async () => await sut.GetPostalInformationDetail(postcode));
        exception.Message.Should().Be(FormattedExceptionMessages.ServiceReturnedNonSuccesfulStatusCode(WellKnownServices.Grar, httpStatusCode, ContextDescription.PostInfoDetail(postcode)));

        httpClient.Verify(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()), Times.Exactly(4));
    }
}
