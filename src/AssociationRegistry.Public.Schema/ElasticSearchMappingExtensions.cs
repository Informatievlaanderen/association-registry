namespace AssociationRegistry.Public.Schema;

using Elastic.Clients.Elasticsearch;
using Search;

public static class ElasticSearchMappingExtensions
{
    public static ElasticsearchClientSettings MapVerenigingDocument(this ElasticsearchClientSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            selector: descriptor => descriptor.IndexName(indexName)
                                              .IdProperty(nameof(VerenigingZoekDocument.VCode)))
                       .DefaultMappingFor(typeof(VerenigingZoekUpdateDocument),
                                          selector: descriptor => descriptor.IndexName(indexName)
                                                                            .IdProperty(nameof(VerenigingZoekUpdateDocument.VCode)));
            ;
    }
}
