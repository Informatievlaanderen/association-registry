namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_A_Set_Of_PostalCodes
{
    [Fact]
    public async Task Then_We_Fetch_The_Nuts_And_Lau_Information_For_Each_Postcode()
    {
        var fixture = new Fixture().CustomizeDomain();
        var client = new Mock<IGrarClient>();
        string[] postcodes = ["1500", "1501"];
        var nutsLauResponses = fixture.CreateMany<PostalNutsLauInfoResponse>(2).ToArray();
        SetupGrarClient(client, postcodes[0], nutsLauResponses[0]);
        SetupGrarClient(client, postcodes[1], nutsLauResponses[1]);

        var sut = new NutsLauFromGrarFetcher(client.Object);

        var actual = await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(postcodes, CancellationToken.None);

        actual.Should().BeEquivalentTo(nutsLauResponses.Select(x => new PostalNutsLauInfo()
        {
            Postcode = x.Postcode,
            Gemeentenaam = x.Gemeentenaam,
            Nuts = x.Nuts,
            Lau = x.Lau,
        }));
    }

    private void SetupGrarClient(Mock<IGrarClient> client, string postalcode, PostalNutsLauInfoResponse response)
    {
        client.Setup(x => x.GetPostalNutsLauInformation(postalcode, It.IsAny<CancellationToken>()))
              .ReturnsAsync(response);
    }
}
