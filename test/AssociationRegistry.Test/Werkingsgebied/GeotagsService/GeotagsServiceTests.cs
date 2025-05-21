namespace AssociationRegistry.Test.Werkingsgebied.GeotagsService;

using AssociationRegistry.Grar.NutsLau;
using Common.Framework;
using FluentAssertions;
using Vereniging.Geotags;
using Xunit;

public class GeotagsServiceTests
{
    private GeotagsService _sut;

    public GeotagsServiceTests()
    {
        var nutsLauInfos = GetPostalNutsLauData();
        var documentStore = TestDocumentStoreFactory.CreateAsync(nameof(GeotagsServiceTests)).GetAwaiter().GetResult();
        using var session = documentStore.LightweightSession();
        session.StoreObjects(nutsLauInfos);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        _sut = new GeotagsService(session);
    }

    [Fact]
    public async Task With_No_MatchingNutsLauInfo_Then_Return_Empty_List_Of_Geotags()
    {
        var nonMatchingPostalCodes = new[]{"1","2"};

        var actual = await _sut.CalculateGeotags(nonMatchingPostalCodes);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task  With_MatchingNutsLauInfo_Then_Return_List_Of_Geotags()
    {
        var matchingPostalCodes = GetPostalNutsLauData().Select(x => x.Postcode).ToArray();

        var actual = await _sut.CalculateGeotags(matchingPostalCodes);

        actual.Should().BeEquivalentTo(GetExpected());
    }

    private List<PostalNutsLauInfo> GetPostalNutsLauData()
        => new()
        {
            new()
            {
                Postcode = "1500",
                Nuts = "BE241",
                Lau = "23027",
            },
            new()
            {
                Postcode = "8000",
                Nuts = "BE251",
                Lau = "31005",
            },
            new()
            {
                Postcode = "9000",
                Nuts = "BE234",
                Lau = "44021",
            },
        };

    private GeoTag[] GetExpected()
        =>
        [
            new("1500"),
            new("BE24123027"),
            new("BE24"),
            new("8000"),
            new("BE25131005"),
            new("BE25"),
            new("9000"),
            new("BE23444021"),
            new("BE23"),
        ];
}
/*
 * GIVEN
 */
