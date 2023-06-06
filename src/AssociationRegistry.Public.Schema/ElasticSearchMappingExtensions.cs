namespace AssociationRegistry.Public.Schema;

using Nest;
using Search;

public static class ElasticSearchMappingExtensions
{
    public static ConnectionSettings MapVerenigingDocument(this ConnectionSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            descriptor => descriptor.IndexName(indexName)
                .IdProperty(nameof(VerenigingZoekDocument.VCode)));
    }
}
