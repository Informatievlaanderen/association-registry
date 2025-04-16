namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_AddressMatches;

using AssociationRegistry.Grar.Clients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Returns_No_Address_Components
{
    [Fact]
    public async Task Then_Returns_Empty_Collection()
    {
        var grarHttpClient = new Mock<IGrarHttpClient>();

        grarHttpClient.Setup(x => x.GetAddressMatches(
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(),
                                 It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                       {
                           Content = new StringContent(AddressMatchResponseWithoutAddressComponents),
                       });

        var sut = new GrarClient(grarHttpClient.Object, new GrarOptions.HttpClientOptions()
        {
            BackoffInMs = [1,1,1],
        }, Mock.Of<ILogger<GrarClient>>());

        var result = await sut.GetAddressMatches(straatnaam: "Aalststraat", huisnummer: "48", busnummer: null, postcode: "1000",
                                                 gemeentenaam: "Brussel", CancellationToken.None);

        result.Should().BeEmpty();
    }

    private const string AddressMatchResponseWithoutAddressComponents =
        @"{
    ""@context"": ""https://docs.basisregisters.staging-vlaanderen.be/context/adresmatch/2023-03-13/adresmatch.jsonld"",
    ""adresMatches"": [
        {
            ""@type"": ""Adres"",
            ""gemeente"": {
                ""objectId"": ""21004"",
                ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/gemeenten/21004"",
                ""gemeentenaam"": {
                    ""geografischeNaam"": {
                        ""spelling"": ""Brussel"",
                        ""taal"": ""nl""
                    }
                }
            },
            ""straatnaam"": {
                ""objectId"": ""19464"",
                ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/straatnamen/19464"",
                ""straatnaam"": {
                    ""geografischeNaam"": {
                        ""spelling"": ""Aalststraat"",
                        ""taal"": ""nl""
                    }
                }
            },
            ""score"": 40.6357199357689,
            ""links"": []
        }
    ],
    ""warnings"": []
    }";
}
