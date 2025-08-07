namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Schema.Search;

public static class ElasticClientExtensions
{
    public static async Task<CreateIndexResponse> CreateVerenigingIndexAsync(this ElasticsearchClient client, string indexName)
    {
        var request = new CreateIndexRequest(indexName)
        {
            Settings = new IndexSettings
            {
                Analysis = new IndexSettingsAnalysis
                {
                    CharFilters = new CharFilters
                    {
                        ["dot_replace"] = new PatternReplaceCharFilter
                        {
                            Pattern = "\\.",
                            Replacement = ""
                        },
                        ["underscore_replace"] = new PatternReplaceCharFilter
                        {
                            Pattern = "_",
                            Replacement = " "
                        }
                    },
                    TokenFilters = new TokenFilters
                    {
                        ["dutch_stop"] = new StopTokenFilter
                        {
                            Stopwords = new Union<StopWordLanguage, ICollection<string>>(StopWordLanguage.Dutch)
                        }
                    },
                    Normalizers = new Normalizers
                    {
                        [VerenigingZoekDocumentMapping.PubliekZoekenNormalizer] = new CustomNormalizer
                        {
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "trim"]
                        }
                    },
                    Analyzers = new Analyzers
                    {
                        [VerenigingZoekDocumentMapping.PubliekZoekenAnalyzer] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "dutch_stop"]
                        }
                    }
                }
            },
            Mappings = VerenigingZoekDocumentMapping.Get()
        };

        var response = await client.Indices.CreateAsync(request);

        if (!response.IsValidResponse)
            throw new Exception("Failed to create Vereniging index: " + response.DebugInformation);

        return response;
    }
}
