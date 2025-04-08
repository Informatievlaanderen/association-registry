namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_The_PostalCodes;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_Grar_Returns_PostalInformationWithout_Volgende
{
    [Fact]
    public async Task Then_Returns_Postcodes()
    {
        var grarClient = new Mock<IGrarClient>();

        string[] postcodes = ["8000", "9000"];

        grarClient.Setup(x => x.GetPostalInformationList("0", "100", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new PostcodesLijstResponse()
                   {
                       Postcodes = postcodes,
                       VolgendeOffset = null,
                       VolgendeLimit = null,
                   });

        var sut = new PostcodesFromGrarFetcher(grarClient.Object);
        var actual = await sut.FetchPostalCodes(CancellationToken.None);
        actual.Should().BeEquivalentTo(postcodes);
    }
}
