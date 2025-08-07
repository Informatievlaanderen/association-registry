namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Schema.Search;

public static class ElasticClientExtensions
{
    public static async Task<CreateIndexResponse> CreateVerenigingIndexAsync(this ElasticsearchClient client, string indexName)
    {
        var request = new CreateIndexRequest(indexName)
        {
            Settings = new IndexSettings
            {
                Analysis = new  IndexSettingsAnalysis
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
                            Pattern = "[_-]",
                            Replacement = " "
                        }
                    },
                    TokenFilters = new TokenFilters
                    {
                        ["dutch_stop"] = new StopTokenFilter
                        {
                            Stopwords = new Union<StopWordLanguage, ICollection<string>>(StopWordLanguage.Dutch),
                        }
                    },
                    Normalizers = new Normalizers
                    {
                        ["beheer_zoeken_normalizer"] = new CustomNormalizer
                        {
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "trim"]
                        }
                    },
                    Analyzers = new Analyzers
                    {
                        ["beheer_zoeken_analyzer"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "dutch_stop"]
                        }
                    }
                }
            },
            Mappings = VerenigingZoekDocumentMapping.Get() // Must return TypeMapping
        };

        var response = await client.Indices.CreateAsync(request);

        if (!response.IsValidResponse)
            throw new Exception("Failed to create Vereniging index: " + response.DebugInformation);

        return response;
    }

    public static async Task<CreateIndexResponse> CreateDuplicateDetectionIndexAsync(this ElasticsearchClient client, string indexName)
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
                            Pattern = "[_-]",
                            Replacement = " "
                        }
                    },
                    TokenFilters = new TokenFilters
                    {
                        ["dutch_stop"] = new StopTokenFilter
                        {
                            Stopwords = new Union<StopWordLanguage, ICollection<string>>(StopWordLanguage.Dutch),
                        },
                        ["synonyms"] = new SynonymTokenFilter
                        {
                            Synonyms = new[] { "st => sint" } // replace with your real synonyms
                        },
                        ["mwd"] = new WordDelimiterTokenFilter
                        {
                            GenerateWordParts = true,
                            CatenateWords = true,
                            PreserveOriginal = false
                        },
                        ["shingle"] = new ShingleTokenFilter
                        {
                            MinShingleSize = 2,
                            MaxShingleSize = 2,
                            OutputUnigrams = true,
                            TokenSeparator = ""
                        }
                    },
                    Analyzers = new Analyzers
                    {
                        ["duplicate_analyzer"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "dutch_stop", "mwd", "shingle"]
                        },
                        ["duplicate_municipality_analyzer"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "dutch_stop"]
                        },
                        ["duplicate_fullname_analyzer"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            CharFilter = ["underscore_replace", "dot_replace"],
                            Filter = ["lowercase", "asciifolding", "dutch_stop", "mwd"]
                        }
                    }
                }
            },
            Mappings = DuplicateDetectionDocumentMapping.Get() // Must return TypeMapping
        };

        var response = await client.Indices.CreateAsync(request);

        if (!response.IsValidResponse)
            throw new Exception("Failed to create Duplicate Detection index: " + response.DebugInformation);

        return response;
    }
}
