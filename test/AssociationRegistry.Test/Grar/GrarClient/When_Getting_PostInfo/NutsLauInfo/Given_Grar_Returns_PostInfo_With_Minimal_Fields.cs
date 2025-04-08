namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.NutsLauInfo;

using AssociationRegistry.Grar.Clients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

public class Given_Grar_Returns_PostInfo_With_Minimal_Fields
{
    [Fact]
    public async Task Then_Returns_Null()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var postcode = "9000";

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                   {
                       Content = new StringContent(PostInfoResponseWithoutGemeenteComponents),
                   });

        var sut = new GrarClient(httpClient.Object, Mock.Of<ILogger<GrarClient>>());

        var actual = await sut.GetPostalNutsLauInformation(postcode, CancellationToken.None);

        actual.Should().BeNull();
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
