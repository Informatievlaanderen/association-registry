namespace AssociationRegistry.Test.Grar.NutsLau.When_Syncing_NutsAndLauInfo;

using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using Xunit;

public class Given_The_Fetchers_Returned_NutsLauInfo
{
    [Fact]
    public async ValueTask Then_Stores_NutsAndLauInfo()
    {

        var postcodesFromGrarFetcher = new Mock<IPostcodesFromGrarFetcher>();
        var nutsLauFromGrarFetcher = new Mock<INutsLauFromGrarFetcher>();

        var postalNutslauInfos = SetupMocks(postcodesFromGrarFetcher, nutsLauFromGrarFetcher);

        var documentSession = new Mock<IDocumentSession>();
        var sut = new NutsAndLauSyncService(nutsLauFromGrarFetcher.Object, postcodesFromGrarFetcher.Object, documentSession.Object, NullLogger<NutsAndLauSyncService>.Instance);

        await sut.SyncNutsLauInfo(CancellationToken.None);

        documentSession.Verify(x => x.DeleteWhere(It.IsAny<Expression<Func<PostalNutsLauInfo, bool>>>()), Times.Once);
        documentSession.Verify(x => x.Store(postalNutslauInfos), Times.Once);
        documentSession.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private PostalNutsLauInfo[] SetupMocks(
        Mock<IPostcodesFromGrarFetcher> postcodesFromGrarFetcher,
        Mock<INutsLauFromGrarFetcher> nutsLauFromGrarFetcher)
    {
        var fixture = new Fixture().CustomizeDomain();
        var postalCodes = fixture.CreateMany<string>().ToArray();
        var postalNutslauInfos = fixture.CreateMany<PostalNutsLauInfo>().ToArray();
        postcodesFromGrarFetcher.Setup(x => x.FetchPostalCodes(It.IsAny<CancellationToken>()))
                                .ReturnsAsync(postalCodes);
        nutsLauFromGrarFetcher.Setup(x => x.GetFlemishAndBrusselsNutsAndLauByPostcode(postalCodes, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(postalNutslauInfos);
        return postalNutslauInfos;
    }
}
