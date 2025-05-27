namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using Moq;

public class NutsLauSetupHelper
{
    public static Mock<IPostcodesFromGrarFetcher> SetupPostCodeFetcher(string[] postCodes)
    {
        var postCodeFetcher = new Mock<IPostcodesFromGrarFetcher>();

        postCodeFetcher.Setup(x => x.FetchPostalCodes(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(postCodes);

        return postCodeFetcher;
    }

    public static void SetupGrarClient(Mock<IGrarClient> client, string postalcode, PostalNutsLauInfoResponse response)
    {
        client.Setup(x => x.GetPostalNutsLauInformation(postalcode, It.IsAny<CancellationToken>()))
              .ReturnsAsync(response);
    }
}
