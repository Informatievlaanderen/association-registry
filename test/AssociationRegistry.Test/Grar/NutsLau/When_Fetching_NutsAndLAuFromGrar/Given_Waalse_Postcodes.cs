namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.NutsLau;
using Moq;
using Xunit;

public class Given_Waalse_Postcodes
{
    [Fact]
    public async ValueTask Then_We_Fetch_The_Nuts_And_Lau_Information_For_Each_Postcode()
    {
        var client = new Mock<IGrarClient>();
        string[] postcodes = ["1300", "4720", "7520"];

        var sut = new NutsLauFromGrarFetcher(client.Object);

        await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(postcodes, CancellationToken.None);

        client.Verify(x => x.GetPostalNutsLauInformation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
