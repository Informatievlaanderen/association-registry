namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_The_PostalCodes;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_Grar_Returns_PostalInformationWith_Volgende
{
    [Fact]
    public async Task Then_Returns_Postcodes_With_A_Next_Batch_Of_Postal_Codes()
    {
        var grarClient = new Mock<IGrarClient>();

        string[] firstBatchPostcodes = ["1500", "1501"];
        var firstBatchOffset = "0";
        var firstBatchLimit = "100";

        string[] nextBatchPostcodes = ["8000", "9000"];
        var nextBatchOffset = "100";
        var nextBatchLimit = "200";

        SetupGrarClient(grarClient, firstBatchOffset, firstBatchLimit, nextBatchOffset, nextBatchLimit, firstBatchPostcodes);
        SetupGrarClient(grarClient, nextBatchOffset, nextBatchLimit, null, null, nextBatchPostcodes);

        var sut = new PostcodesFromGrarFetcher(grarClient.Object);
        var actual = await sut.FetchPostalCodes();

        actual.Should().BeEquivalentTo(firstBatchPostcodes.Concat(nextBatchPostcodes));
    }

    private static void SetupGrarClient(Mock<IGrarClient> grarClient, string offset, string limit, string nextOffset, string nextLimit, string[] batchPostcodes)
    {
        grarClient.Setup(x => x.GetPostalInformationList(offset, limit))
                  .ReturnsAsync(new PostcodesLijstResponse()
                   {
                       Postcodes = batchPostcodes,
                       VolgendeOffset = nextOffset,
                       VolgendeLimit = nextLimit,
                   });
    }
}
