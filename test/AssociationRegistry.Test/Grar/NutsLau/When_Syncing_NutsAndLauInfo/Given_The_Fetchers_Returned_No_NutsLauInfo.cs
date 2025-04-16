namespace AssociationRegistry.Test.Grar.NutsLau.When_Syncing_NutsAndLauInfo;

using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using Xunit;

public class Given_The_Fetchers_Returned_No_NutsLauInfo
{
    [Fact]
    public async Task Then_Keep_Previous_NutsLauInfo()
    {

        var postcodesFromGrarFetcher = new Mock<IPostcodesFromGrarFetcher>();
        var nutsLauFromGrarFetcher = new Mock<INutsLauFromGrarFetcher>();

        SetupMocks(postcodesFromGrarFetcher, nutsLauFromGrarFetcher);

        var documentSession = new Mock<IDocumentSession>();
        var sut = new NutsAndLauSyncService(nutsLauFromGrarFetcher.Object, postcodesFromGrarFetcher.Object, documentSession.Object, NullLogger<NutsAndLauSyncService>.Instance);

        await sut.SyncNutsLauInfo(CancellationToken.None);

        documentSession.Verify(x => x.DeleteWhere(It.IsAny<Expression<Func<PostalNutsLauInfo, bool>>>()), Times.Never);
        documentSession.Verify(x => x.Store(It.IsAny<PostalNutsLauInfo>()), Times.Never);
        documentSession.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private void SetupMocks(
        Mock<IPostcodesFromGrarFetcher> postcodesFromGrarFetcher,
        Mock<INutsLauFromGrarFetcher> nutsLauFromGrarFetcher)
    {
        var fixture = new Fixture().CustomizeDomain();
        var postalCodes = fixture.CreateMany<string>().ToArray();
        postcodesFromGrarFetcher.Setup(x => x.FetchPostalCodes(It.IsAny<CancellationToken>()))
                                .ReturnsAsync(postalCodes);
        nutsLauFromGrarFetcher.Setup(x => x.GetFlemishAndBrusselsNutsAndLauByPostcode(postalCodes, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(Array.Empty<PostalNutsLauInfo>());
    }
}
