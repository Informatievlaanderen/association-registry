namespace AssociationRegistry.Test.Projections.Beheer.ZoekDocumentMapping;

using AssociationRegistry.Admin.Schema.Search;
using Elastic.Clients.Elasticsearch.Mapping;
using FluentAssertions;
using Xunit;

public class VerenigingZoekDocumentMappingTests
{
    [Fact]
    public void Mapping_Should_Include_Lidmaatschappen_With_Text_Fields_Instead_Of_Date_Fields()
    {
        var mapping = VerenigingZoekDocumentMapping.Get();

        mapping.Properties.Should().ContainKey("lidmaatschappen");
        var lidmaatschappenProperty = mapping.Properties["lidmaatschappen"] as NestedProperty;
        lidmaatschappenProperty.Should().NotBeNull();

        var lidmaatschappenProperties = lidmaatschappenProperty!.Properties;
        lidmaatschappenProperties.Should().ContainKey("datumVan");
        lidmaatschappenProperties.Should().ContainKey("datumTot");

        lidmaatschappenProperties["datumVan"].Should().BeOfType<TextProperty>();
        lidmaatschappenProperties["datumTot"].Should().BeOfType<TextProperty>();
    }

    [Fact]
    public void Mapping_Should_Not_Have_Date_Fields_For_Lidmaatschap_Dates()
    {
        var mapping = VerenigingZoekDocumentMapping.Get();

        var lidmaatschappenProperty = mapping.Properties["lidmaatschappen"] as NestedProperty;
        var lidmaatschappenProperties = lidmaatschappenProperty!.Properties;

        // These should NOT be DateProperty instances
        lidmaatschappenProperties["datumVan"].Should().NotBeOfType<DateProperty>();
        lidmaatschappenProperties["datumTot"].Should().NotBeOfType<DateProperty>();
    }
}
