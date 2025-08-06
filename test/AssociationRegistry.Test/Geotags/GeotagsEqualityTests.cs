namespace AssociationRegistry.Test.Geotags;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Geotags;
using FluentAssertions;
using Xunit;

public class GeotagsEqualityTests
{
    [Fact]
    public void Given_Two_Identical_Lists_Of_Geotags_Then_Are_Equal()
    {
        var fixture = new Fixture().CustomizeDomain();

        var geotags1 = fixture.CreateMany<Geotag>().ToList();
        var geotags2 = geotags1.Select(x => new Geotag(x.Identificatie));
        var geotagsCollection1 = GeotagsCollection.CreateInstance(geotags1);
        var geotagsCollection2 = GeotagsCollection.CreateInstance(geotags2.ToList());

        geotagsCollection1.Equals(geotagsCollection2).Should().BeTrue();
        (geotagsCollection1 == geotagsCollection2).Should().BeTrue();
    }

    [Fact]
    public void Given_Two_Empty_Lists_Of_Geotags_Then_Are_Equal()
    {
        var geotagsCollection1 = GeotagsCollection.CreateInstance([]);
        var geotagsCollection2 = GeotagsCollection.CreateInstance([]);

        geotagsCollection1.Equals(geotagsCollection2).Should().BeTrue();
        (geotagsCollection1 == geotagsCollection2).Should().BeTrue();
    }

    [Fact]
    public void Equals_SameUniqueItemsDifferentOrder_ReturnsTrue()
    {
        var items1 = new List<Geotag>
        {
            new Geotag("Alpha"),
            new Geotag("Beta"),
            new Geotag("Gamma")
        };
        var items2 = new List<Geotag>
        {
            new Geotag("Gamma"),
            new Geotag("Alpha"),
            new Geotag("Beta")
        };
        var a = GeotagsCollection.CreateInstance(items1);
        var b = GeotagsCollection.CreateInstance(items2);

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_OneHasExtraUniqueItem_ReturnsFalse()
    {
        var a = GeotagsCollection.CreateInstance(new[]
        {
            new Geotag("A"),
            new Geotag("B")
        });
        var b = GeotagsCollection.CreateInstance(new[]
        {
            new Geotag("A"),
            new Geotag("B"),
            new Geotag("C")
        });
        // Unique sets are {A,B} vs {A,B,C} → not equal
        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
    }

    [Fact]
    public void Equals_DifferentUniqueItems_ReturnsFalse()
    {
        var a = GeotagsCollection.CreateInstance(new[]
        {
            new Geotag("X"),
            new Geotag("Y"),
            new Geotag("Z")
        });
        var b = GeotagsCollection.CreateInstance(new[]
        {
            new Geotag("X"),
            new Geotag("Y"),
            new Geotag("Different")
        });
        // Unique sets {X,Y,Z} vs {X,Y,Different} differ at "Z" vs "Different"
        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
    }
}
