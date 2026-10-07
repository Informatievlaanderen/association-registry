namespace AssociationRegistry.Test.Grar.NutsLau;

using AssociationRegistry.Grar.NutsLau;
using Common.Framework;
using FluentAssertions;
using Marten;
using Xunit;

public class PostcodesInSameGemeenteServiceTests : IAsyncLifetime
{
    private IDocumentStore _store = null!;
    private IDocumentSession _session = null!;
    private PostcodesInSameGemeenteService _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _store = await TestDocumentStoreFactory.CreateAsync(nameof(PostcodesInSameGemeenteServiceTests));
        _session = _store.LightweightSession();

        _session.Store(
            Info("1500", "Halle", "BE24", "23027"),
            Info("1501", "Buizingen", "BE24", "23027"),
            Info("1502", "Lembeek", "BE24", "23027"),
            Info("9000", "Gent", "BE23", "44021"),
            Info("9040", "Sint-Amandsberg", "BE23", "44021"),
            // same Lau code, different Nuts3: must not be mixed up
            Info("2000", "Anderstad", "BE21", "23027")
        );

        await _session.SaveChangesAsync();

        _sut = new PostcodesInSameGemeenteService(_session);
    }

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync();
        _store.Dispose();
    }

    [Fact]
    public async ValueTask Given_Postcode_Then_Returns_All_Postcodes_Of_Same_Gemeente()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1500"]);

        actual.Should().BeEquivalentTo("1500", "1501", "1502");
    }

    [Fact]
    public async ValueTask Given_Secondary_Postcode_Then_Returns_All_Postcodes_Of_Same_Gemeente()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1502"]);

        actual.Should().BeEquivalentTo("1500", "1501", "1502");
    }

    [Fact]
    public async ValueTask Given_Postcodes_Of_Same_Gemeente_Then_Returns_Distinct_List()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1500", "1501"]);

        actual.Should().BeEquivalentTo("1500", "1501", "1502");
        actual.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async ValueTask Given_Duplicate_Input_Postcodes_Then_Returns_Distinct_List()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1500", "1500"]);

        actual.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async ValueTask Given_Postcodes_Of_Different_Gemeentes_Then_Returns_Union()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1500", "9000"]);

        actual.Should().BeEquivalentTo("1500", "1501", "1502", "9000", "9040");
    }

    [Fact]
    public async ValueTask Given_Gemeente_With_Same_Lau_But_Other_Nuts3_Then_It_Is_Not_Included()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["1500"]);

        actual.Should().NotContain("2000");
    }

    [Fact]
    public async ValueTask Given_Unknown_Postcode_Then_Returns_Input_Postcode()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["0000"]);

        actual.Should().BeEquivalentTo("0000");
    }

    [Fact]
    public async ValueTask Given_Known_And_Unknown_Postcode_Then_Keeps_Unknown_And_Expands_Known()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente(["0000", "1500"]);

        actual.Should().BeEquivalentTo("0000", "1500", "1501", "1502");
    }

    [Fact]
    public async ValueTask Given_No_Postcodes_Then_Returns_Empty()
    {
        var actual = await _sut.WithPostcodesOfSameGemeente([]);

        actual.Should().BeEmpty();
    }

    private static PostalNutsLauInfo Info(string postcode, string gemeente, string nuts3, string lau) =>
        new()
        {
            Postcode = postcode,
            Gemeentenaam = gemeente,
            Nuts3 = nuts3,
            Lau = lau,
        };
}
