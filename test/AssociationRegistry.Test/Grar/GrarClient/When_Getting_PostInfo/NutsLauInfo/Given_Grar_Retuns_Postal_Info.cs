namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.NutsLauInfo;

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

public class Given_Grar_Retuns_Postal_Info
{
    [Fact]
    public async Task Then_Returns_Nuts_Lau_Info()
    {
        var httpClient = new Mock<IGrarHttpClient>();
        var postcode = "9000";
        var lauCode = "12345";
        var gemeentenaam = "Gent";
        var nutsCode = "BE242";

        var postalInfoResponse = new PostalInformationOsloResponseBuilder()
                                .withPostcode(postcode)
                                .WithLau(lauCode)
                                .WithGemeente(gemeentenaam)
                                .WithNuts(nutsCode)
                                .Build();

        SetupHttpClientMockToReturnPostInfoResponse(postalInfoResponse, httpClient, postcode);

        var sut = new GrarClient(httpClient.Object, new GrarOptions.HttpClientOptions()
        {
            BackoffInMs = [1,1,1],
        }, Mock.Of<ILogger<GrarClient>>());

        var expected = new PostalNutsLauInfoResponse(postcode, gemeentenaam, nutsCode, lauCode);

        var actual = await sut.GetPostalNutsLauInformation(postcode, CancellationToken.None);

        actual.Should().BeEquivalentTo(expected);
    }

    private static void SetupHttpClientMockToReturnPostInfoResponse(
        PostalInformationOsloResponse postalInfoResponse,
        Mock<IGrarHttpClient> httpClient,
        string postcode)
    {
        var json = JsonConvert.SerializeObject(postalInfoResponse);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        httpClient.Setup(x => x.GetPostInfoDetail(postcode, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                   {
                       Content = content,
                   });
    }
}
