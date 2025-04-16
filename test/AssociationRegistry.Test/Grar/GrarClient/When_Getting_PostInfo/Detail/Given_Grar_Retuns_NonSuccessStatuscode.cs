namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.Detail;

using AssociationRegistry.Grar.Clients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Retuns_NonSuccessStatuscode
{
    [Fact]
    public async Task Then_Throws_Exception()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var postcode = "9000";

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var sut = new GrarClient(httpClient.Object, Mock.Of<ILogger<GrarClient>>());

        var exception = await Assert.ThrowsAsync<Exception>(async () => await sut.GetPostalInformationDetail(postcode));
        exception.Message.Should().Be($"grar returned {HttpStatusCode.BadRequest} for PostInfoDetail with postcode {postcode}");
    }
}
