namespace AssociationRegistry.Test.GrarClient.When_Getting_PostInfo;

using AssociationRegistry.Grar;
using FluentAssertions;
using Grar.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Returns_No_Gemeente_Components
{
    [Fact]
    public async Task Then_Returns_Empty_Collection()
    {
        var grarHttpClient = new Mock<IGrarHttpClient>();

        grarHttpClient.Setup(x => x.GetPostInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                       {
                           Content = new StringContent(PostInfoResponseWithoutGemeenteComponents),
                       });

        var sut = new GrarClient(grarHttpClient.Object, Mock.Of<ILogger<GrarClient>>());

        var result = await sut.GetPostalInformation("0612");

        result.Should().BeEquivalentTo(new PostalInformationResponse("0612", "Sinterklaas", new [] { "Sinterklaas" }));
    }

    private const string PostInfoResponseWithoutGemeenteComponents =
        @"{
          ""@context"": ""https://docs.basisregisters.test-vlaanderen.be/context/postinfo/2023-10-16/postinfo_detail.jsonld"",
          ""@type"": ""PostInfo"",
          ""identificator"": {
            ""id"": ""https://data.vlaanderen.be/id/postinfo/0612"",
            ""naamruimte"": ""https://data.vlaanderen.be/id/postinfo"",
            ""objectId"": ""0612"",
            ""versieId"": ""2020-02-10T12:42:50+01:00""
          },
          ""postnamen"": [
            {
              ""geografischeNaam"": {
                ""spelling"": ""Sinterklaas"",
                ""taal"": ""nl""
              }
            }
          ],
          ""postInfoStatus"": ""gerealiseerd""
        }";
}
