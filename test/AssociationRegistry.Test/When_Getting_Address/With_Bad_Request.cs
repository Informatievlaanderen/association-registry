﻿namespace AssociationRegistry.Test.When_Getting_Address;

using Grar;
using Grar.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class With_Bad_Request
{
    [Fact]
    public async Task Then_Throws_AdressenregisterReturnedNonSuccessStatusCode()
    {
        var grarHttpClient = new Mock<IGrarHttpClient>();

        grarHttpClient.Setup(x => x.GetAddress(
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var sut = new GrarClient(grarHttpClient.Object, Mock.Of<ILogger<GrarClient>>());

        await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(
            () => sut.GetAddressMatches("straatnaam", "nr", null, "postcode", "gemeentenaam"));
    }
}
