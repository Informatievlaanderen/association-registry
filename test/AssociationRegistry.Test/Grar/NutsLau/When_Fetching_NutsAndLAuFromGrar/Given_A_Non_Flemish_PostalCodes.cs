namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.NutsLau;
using Moq;
using Xunit;

public class Given_A_Non_Flemish_PostalCodes
{
    [Fact]
    public async Task Then_We_Fetch_The_Nuts_And_Lau_Information_For_Each_Postcode()
    {
        var client = new Mock<IGrarClient>();
        string[] postcodes = ["1000", "4720"];

        var sut = new NutsLauFromGrarFetcher(client.Object);

        await sut.GetFlemishNutsAndLauByPostcode(postcodes, CancellationToken.None);

        client.Verify(x => x.GetPostalNutsLauInformation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
