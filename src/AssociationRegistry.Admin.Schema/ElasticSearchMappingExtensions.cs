namespace AssociationRegistry.Admin.Schema;

using Elastic.Clients.Elasticsearch;
using Search;

public static class ElasticSearchMappingExtensions
{
    private static ElasticsearchClientSettings MapVerenigingZoekDocument(this ElasticsearchClientSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            selector: descriptor => descriptor.IndexName(indexName)
                                              .IdProperty(nameof(VerenigingZoekDocument.VCode)));
    }

    private static ElasticsearchClientSettings MapVerenigingDuplicationDocument(this ElasticsearchClientSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(DuplicateDetectionDocument),
            selector: descriptor => descriptor.IndexName(indexName)
                                              .IdProperty(nameof(DuplicateDetectionDocument.VCode)));
    }

    public static ElasticsearchClientSettings MapAllVerenigingDocuments(this ElasticsearchClientSettings settings, string zoekIndexName, string duplicationIndexName)
        => settings.MapVerenigingZoekDocument(zoekIndexName)
                   .MapVerenigingDuplicationDocument(duplicationIndexName);
}
