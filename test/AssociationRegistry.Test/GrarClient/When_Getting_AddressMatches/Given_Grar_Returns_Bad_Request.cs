namespace AssociationRegistry.Test.GrarClient.When_Getting_AddressMatches;

using Grar;
using Grar.Clients;
using Grar.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Returns_Bad_Request
{
    [Fact]
    public async Task Then_Throws_AdressenregisterReturnedNonSuccessStatusCode()
    {
        var grarHttpClient = new Mock<IGrarHttpClient>();

        grarHttpClient.Setup(x => x.GetAddressMatches(
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var sut = new GrarClient(grarHttpClient.Object, Mock.Of<ILogger<GrarClient>>());

        await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(
            () => sut.GetAddressMatches(straatnaam: "straatnaam", huisnummer: "nr", busnummer: null, postcode: "postcode",
                                        gemeentenaam: "gemeentenaam", CancellationToken.None));
    }
}
