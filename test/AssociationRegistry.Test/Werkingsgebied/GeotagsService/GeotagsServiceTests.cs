namespace AssociationRegistry.Test.Werkingsgebied.GeotagsService;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Vereniging;
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
    public async Task With_Locaties_And_Werkingsgebieden_Then_Returns_List_Of_Matching_Geotags()
    {
        var fixture = new Fixture().CustomizeDomain();

        Werkingsgebied[] matchingProvincieWerkingsgebied = [Werkingsgebied.Hydrate("BE24224055", "Kortenberg")];
        Locatie[] matchingPostalCodes = [fixture.Create<Locatie>() with
        {
            Adres = fixture.Create<Adres>() with
            {
                Postcode = "1500",
            },
        }];

        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24", "BE24123027", "1500");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, matchingProvincieWerkingsgebied);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_No_MatchingNutsLauInfo_Then_Return_Empty_List_Of_Geotags()
    {
        var nonMatchingPostalCodes = new[]{"1","2"};

        var actual = await _sut.CalculateGeotags(nonMatchingPostalCodes, []);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task With_One_Matching_PostalCode_Then_Return_Their_Geotags()
    {
        var matchingPostalCodes = new[]{"3070"};
        var expectedGeotags = GeotagFrom("3070", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, []);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task  With_2_Matching_PostalCodes_In_Same_Provincie_Then_Return_A_Distinct_Of_Their_Geotags()
    {
        var matchingPostalCodes = new[]{"3070", "3071"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, []);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_One_Matching_Gemeente_Werkingsgebied_Then_Return_Their_Geotags()
    {
        var matchingMunicipalityWerkingsgebied = new[]{"BE24224055"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24");

        var actual = await _sut.CalculateGeotags([], matchingMunicipalityWerkingsgebied);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_Two_Matching_Gemeente_Werkingsgebied_In_Same_Provincie_Then_Return_Their_Geotags()
    {
        var matchingMunicipalityWerkingsgebied = new[]{"BE24224055", "BE24123027"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24", "BE24123027", "1500", "1501", "1502");

        var actual = await _sut.CalculateGeotags([], matchingMunicipalityWerkingsgebied);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_Matching_Provincie_Werkingsgebied_Then_Return_Their_Geotags()
    {
        var matchingProvincieWerkingsgebied = new[]{"BE24"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24", "BE24123027", "1500", "1501", "1502");

        var actual = await _sut.CalculateGeotags([], matchingProvincieWerkingsgebied);

        actual.Should().BeEquivalentTo(expectedGeotags);
    }

    [Fact]
    public async Task With_Mix_Of_Werkingsgebieden_And_PostalCodes_Then_Return_Their_Geotags()
    {
        var matchingProvincieWerkingsgebied = new[]{"BE24224055"};
        var matchingPostalCodes = new[]{"1500", "9030"};
        var expectedGeotags = GeotagFrom("3070", "3071", "BE24224055", "BE24", "BE24123027", "1500", "9030", "BE23", "BE23444021");

        var actual = await _sut.CalculateGeotags(matchingPostalCodes, matchingProvincieWerkingsgebied);

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
}
