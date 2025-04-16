namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.List;

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
        var offset = "100";
        var limit = "200";

        httpClient.Setup(x => x.GetPostInfoList(offset, limit, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var sut = new GrarClient(httpClient.Object, Mock.Of<ILogger<GrarClient>>());

        var exception = await Assert.ThrowsAsync<Exception>(async () => await sut.GetPostalInformationList(offset, limit, CancellationToken.None));
        exception.Message.Should().Be($"grar returned {HttpStatusCode.BadRequest} for PostInfoLijst with offset {offset} and limit {limit}");
    }
}
