namespace AssociationRegistry.Test.Geotags;

using FluentAssertions;
using Vereniging.Geotags;
using Xunit;

public class GeotagsCollectionTests
{
    [Fact]
    public void Given_DuplicateItems_Then_Disctinct()
    {
        var geotagsCollection1 = GeotagsCollection.CreateInstance([new Geotag("1"), new Geotag("1"), new Geotag("2")]);
        geotagsCollection1.Should().BeEquivalentTo(GeotagsCollection.CreateInstance([new Geotag("1"), new Geotag("2")]));
    }
}
