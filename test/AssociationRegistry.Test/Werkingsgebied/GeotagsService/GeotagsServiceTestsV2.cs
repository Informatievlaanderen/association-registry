namespace AssociationRegistry.Test.Werkingsgebied.GeotagsService;

using AssociationRegistry.Grar.NutsLau;
using Common.Framework;
using FluentAssertions;
using Vereniging.Geotags;
using Xunit;

public class GeotagsServiceTestsV2
{
    private GeotagsService _sut;

    public GeotagsServiceTestsV2()
    {
        var nutsLauInfos = GetPostalNutsLauData();
        var documentStore = TestDocumentStoreFactory.CreateAsync(nameof(GeotagsServiceTestsV2)).GetAwaiter().GetResult();
        using var session = documentStore.LightweightSession();
        session.StoreObjects(nutsLauInfos);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        _sut = new GeotagsService(session);
    }

    [Fact]
    public async Task With_No_MatchingNutsLauInfo_Then_Return_Empty_List_Of_Geotags()
    {
        var nonMatchingPostalCodes = new[]{"1","2"};

        var actual = await _sut.CalculateGeotags(nonMatchingPostalCodes, []);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task  With_One_Matching_PostalCode_Then_Return_Their_Geotags()
    {
        var matchingPostalCodes = new[]{"3070"};
        var expectedGeotags = GeotagFrom("3070", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, []);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task  With_2_Matching_PostalCodes_In_Same_Region_Then_Return_A_Distinct_Of_Their_Geotags()
    {
        var matchingPostalCodes = new[]{"3070", "3071"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, []);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_One_Matching_Municipality_Werkingsgebied_Then_Return_Their_Geotags()
    {
        var matchingMunicipalityWerkingsgebied = new[]{"BE24224055"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags([], matchingMunicipalityWerkingsgebied);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    private GeoTag[] GeotagFrom(params string[] geotags)
        => geotags.Select(x => new GeoTag(x)).ToArray();

    private List<PostalNutsLauInfo> GetPostalNutsLauData()
        => new()
        {
            new PostalNutsLauInfo() { Lau = "23027", Nuts = "BE241", Postcode= "1500", Gemeentenaam = "Halle" },
            new PostalNutsLauInfo() { Lau = "23027", Nuts = "BE241", Postcode= "1501", Gemeentenaam = "Halle" },
            new PostalNutsLauInfo() { Lau = "23027", Nuts = "BE241", Postcode= "1502", Gemeentenaam = "Halle" },

            new PostalNutsLauInfo() { Lau = "24055", Nuts = "BE242", Postcode= "3070", Gemeentenaam = "Kortenberg" },
            new PostalNutsLauInfo() { Lau = "24055", Nuts = "BE242", Postcode= "3071", Gemeentenaam = "Kortenberg" },

            new PostalNutsLauInfo() { Lau = "44021", Nuts = "BE234", Postcode= "9000", Gemeentenaam = "Gent" },
            new PostalNutsLauInfo() { Lau = "44021", Nuts = "BE234", Postcode= "9030", Gemeentenaam = "Gent" },
            new PostalNutsLauInfo() { Lau = "44021", Nuts = "BE234", Postcode= "9031", Gemeentenaam = "Gent" },
            new PostalNutsLauInfo() { Lau = "44021", Nuts = "BE234", Postcode= "9032", Gemeentenaam = "Gent" },
            new PostalNutsLauInfo() { Lau = "44021", Nuts = "BE234", Postcode= "9040", Gemeentenaam = "Gent" },
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
