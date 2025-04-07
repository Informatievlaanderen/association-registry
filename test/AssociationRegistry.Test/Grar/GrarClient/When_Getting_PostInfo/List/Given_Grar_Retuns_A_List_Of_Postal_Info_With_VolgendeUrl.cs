namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.List;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Clients.Contracts;
using AssociationRegistry.Grar.Models.PostalInfo;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

public class Given_Grar_Retuns_A_List_Of_Postal_Info_With_VolgendeUrl
{
    [Fact]
    public async Task Then_Returns_PostalInfoList_With_Offset_And_Limit()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var offset = "100";
        var limit = "50";
        var volgendeOffset = "100";
        var volgendeLimit = "50";

        var postalInfoResponse = new PostalInformationListOsloResponseBuilder()
                          .WithPostInfo("1500", "Halle", "Buizingen")
                          .WithPostInfo("9000", "Gent")
                          .WithVolgende($"https://api.example.com/postinfo?offset={volgendeOffset}&limit={volgendeLimit}")
                          .Build();

        SetupHttpClientMockToReturnPostInfoResponse(postalInfoResponse, httpClient, offset, limit);

        var sut = new GrarClient(httpClient.Object, Mock.Of<ILogger<GrarClient>>());


        var expected = new PostcodesLijstResponse()
        {
            Postcodes = postalInfoResponse.PostInfoObjecten.Select(x => x.Identificator.ObjectId).ToArray(),
            VolgendeOffset = volgendeOffset,
            VolgendeLimit = volgendeLimit,
        };

        var actual = await sut.GetPostalInformationList(offset, limit);

        actual.Should().BeEquivalentTo(expected);
    }

    private static void SetupHttpClientMockToReturnPostInfoResponse(
        PostalInformationListOsloResponse postalInfoResponse,
        Mock<IGrarHttpClient> httpClient,
        string offset,
        string limit)
    {
        var json = JsonConvert.SerializeObject(postalInfoResponse);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        httpClient.Setup(x => x.GetPostInfoList(offset, limit, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                   {
                       Content = content,
                   });
    }
}
