namespace AssociationRegistry.Test.GrarClient.When_Getting_AddressMatches;

using FluentAssertions;
using Grar;
using Grar.Clients;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Returns_A_Null_BoxNumber
{
    [Fact]
    public async Task Then_Returns_Empty_String_BoxNumber()
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
                           Content = new StringContent(AddressMatchResponseWithoutBoxNumber),
                       });

        var sut = new GrarClient(grarHttpClient.Object, Mock.Of<ILogger<GrarClient>>());

        var result = await sut.GetAddressMatches(straatnaam: "Fosselstraat", huisnummer: "48", busnummer: null, postcode: "1790",
                                                 gemeentenaam: "Affligem", CancellationToken.None);

        result.First().Busnummer.Should().NotBeNull();
        result.First().Busnummer.Should().BeEmpty();
    }

    private const string AddressMatchResponseWithoutBoxNumber =
        @"   { ""@context"": ""https://docs.basisregisters.staging-vlaanderen.be/context/adresmatch/2023-03-13/adresmatch.jsonld"",
    ""adresMatches"": [
      {
        ""@type"": ""Adres"",
        ""identificator"": {
          ""id"": ""https://data.vlaanderen.be/id/adres/2208355"",
          ""naamruimte"": ""https://data.vlaanderen.be/id/adres"",
          ""objectId"": ""2208355"",
          ""versieId"": ""2022-05-01T03:11:38+02:00""
        },
        ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/adressen/2208355"",
        ""gemeente"": {
          ""objectId"": ""23105"",
          ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/gemeenten/23105"",
          ""gemeentenaam"": {
            ""geografischeNaam"": {
              ""spelling"": ""Affligem"",
              ""taal"": ""nl""
            }
          }
        },
        ""postinfo"": {
          ""objectId"": ""1790"",
          ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/postinfo/1790""
        },
        ""straatnaam"": {
          ""objectId"": ""31402"",
          ""detail"": ""https://api.basisregisters.staging-vlaanderen.be/v2/straatnamen/31402"",
          ""straatnaam"": {
            ""geografischeNaam"": {
              ""spelling"": ""Fosselstraat"",
              ""taal"": ""nl""
            }
          }
        },
        ""huisnummer"": ""48"",
        ""volledigAdres"": {
          ""geografischeNaam"": {
            ""spelling"": ""Fosselstraat 48, 1790 Affligem"",
            ""taal"": ""nl""
          }
        },
        ""adresPositie"": {
          ""geometrie"": {
            ""type"": ""Point"",
            ""gml"": ""\u003Cgml:Point srsName=\""https://www.opengis.net/def/crs/EPSG/0/31370\"" xmlns:gml=\""http://www.opengis.net/gml/3.2\""\u003E\u003Cgml:pos\u003E132017.28 178609.15\u003C/gml:pos\u003E\u003C/gml:Point\u003E""
          },
          ""positieGeometrieMethode"": ""afgeleidVanObject"",
          ""positieSpecificatie"": ""gebouweenheid""
        },
        ""adresStatus"": ""inGebruik"",
        ""officieelToegekend"": true,
        ""score"": 100,
        ""links"": [
          {
            ""href"": ""https://api.basisregisters.staging-vlaanderen.be/v2/percelen?adresobjectid=2208355"",
            ""rel"": ""percelen"",
            ""type"": ""GET""
          },
          {
            ""href"": ""https://api.basisregisters.staging-vlaanderen.be/v2/gebouweenheden?adresobjectid=2208355"",
            ""rel"": ""gebouweenheden"",
            ""type"": ""GET""
          }
        ]
      }]}";
}
