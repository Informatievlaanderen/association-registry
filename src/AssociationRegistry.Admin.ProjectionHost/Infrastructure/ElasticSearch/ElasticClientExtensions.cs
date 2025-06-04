namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;

using Schema.Search;
using Nest;
using Nest.Specification.IndicesApi;

public static class ElasticClientExtensions
{
    private const string TokenFilterWordDelimiter = "mwd";
    private const string TokenFilterShingle = "shingle";
    private const string TokenFilterMunicipalities = "muni";
    private const string TokenFilterSynonyms = "synonyms";
    private const string TokenFilterDutchStop = "dutch_stop";
    private const string CharFilterDotReplace = "dot_replace";
    private const string CharFilterUnderscoreReplace = "underscore_replace";

    public static CreateIndexResponse CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => CreateVerenigingIndexAsync(indicesNamespace, index).GetAwaiter().GetResult();

    public static async Task<CreateIndexResponse> CreateVerenigingIndexAsync(this IndicesNamespace indicesNamespace, IndexName index)
        => await indicesNamespace.CreateAsync(
            index,
            selector: descriptor =>
                descriptor
                   .Settings(s => s
                                .Analysis(a => a.CharFilters(cf => cf.RemoveDots()
                                                                     .ReplaceUnderscoresAndHyphenWithSpaces())
                                                .TokenFilters(FilterDutchStopWords)
                                                .Normalizers(AddVerenigingZoekNormalizer)
                                                .Analyzers(AddVerenigingZoekAnalyzer)))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

    public static void CreateDuplicateDetectionIndex(this IndicesNamespace indicesNamespace, IndexName index)
    {
        CreateDuplicateDetectionIndexAsync(indicesNamespace, index).GetAwaiter().GetResult();
    }

    public static async Task<CreateIndexResponse> CreateDuplicateDetectionIndexAsync(
        this IndicesNamespace indicesNamespace,
        IndexName index)
    {
        var createIndexResponse = await indicesNamespace.CreateAsync(
            index,
            selector: c => c
                          .Settings(s => s
                                       .Analysis(a => a
                                                     .CharFilters(cf => cf.RemoveDots()
                                                                          .ReplaceUnderscoresAndHyphenWithSpaces())
                                                     .Analyzers(AddDuplicateDetectionAnalyzer)
                                                     .TokenFilters(tf => tf.FilterDutchStopWords()
                                                                           .UseWordDelimiter()
                                                                           .CombineNeighbouringWordsWithShingle()
                                                      )))
                          .Map<DuplicateDetectionDocument>(DuplicateDetectionDocumentMapping.Get));

        if (!createIndexResponse.IsValid)
            throw createIndexResponse.OriginalException;

        return createIndexResponse;
    }

    private static AnalyzersDescriptor AddDuplicateDetectionAnalyzer(AnalyzersDescriptor ad)
        => ad.Custom(DuplicateDetectionDocumentMapping.DuplicateAnalyzer,
                     selector: ca
                         => ca
                           .Tokenizer("standard")
                           .CharFilters(CharFilterUnderscoreReplace, CharFilterDotReplace)
                           .Filters("lowercase", "asciifolding", TokenFilterDutchStop, TokenFilterWordDelimiter, TokenFilterShingle)
        ).Custom(DuplicateDetectionDocumentMapping.DuplicateMunicipalityAnalyzer,
                 selector: ca
                     => ca
                       .Tokenizer("standard")
                       .CharFilters(CharFilterUnderscoreReplace, CharFilterDotReplace)
                       .Filters("lowercase", "asciifolding", TokenFilterDutchStop)
        ).Custom(DuplicateDetectionDocumentMapping.DuplicateFullNameAnalyzer,
                 selector: ca
                     => ca
                       .Tokenizer("standard")
                       .CharFilters(CharFilterUnderscoreReplace, CharFilterDotReplace)
                       .Filters("lowercase", "asciifolding", TokenFilterDutchStop, TokenFilterWordDelimiter)
        );


    private static TokenFiltersDescriptor FilterDutchStopWords(this TokenFiltersDescriptor tf)
    {
        return tf.Stop(name: TokenFilterDutchStop, selector: st => st
                          .StopWords("_dutch_") // Or provide your custom list
        );
    }

    private static TokenFiltersDescriptor FilterMunicipalities(this TokenFiltersDescriptor tf, string[] municipalities)
    {
        return tf.Stop(name: TokenFilterMunicipalities, selector: st => st
                          .StopWords(municipalities) // Or provide your custom list
        );
    }

    private static TokenFiltersDescriptor AddSynonyms(this TokenFiltersDescriptor tf)
    {
        return tf.Synonym(name: TokenFilterSynonyms, selector: st => st
                          .Synonyms("st => sint") // Or provide your custom list
        );
    }

    private static CharFiltersDescriptor RemoveDots(this CharFiltersDescriptor cf)
    {
        return cf.PatternReplace(name: CharFilterDotReplace,
                                 selector: prcf
                                     => prcf.Pattern("\\.").Replacement(""));

    }

    private static CharFiltersDescriptor ReplaceUnderscoresAndHyphenWithSpaces(this CharFiltersDescriptor cf)
    {
        return cf.PatternReplace(name: CharFilterUnderscoreReplace,
                                 selector: prcf => prcf
                                                  .Pattern("[_-]")
                                                  .Replacement(" "));
    }

    public static TokenFiltersDescriptor CombineNeighbouringWordsWithShingle(this TokenFiltersDescriptor source)
    {
        return source.Shingle(TokenFilterShingle, sd => sd.MinShingleSize(2)
                                                          .MaxShingleSize(2)
                                                          .OutputUnigrams(true)
                                                          .TokenSeparator(""));
    }

    public static TokenFiltersDescriptor UseWordDelimiter(this TokenFiltersDescriptor source)
    {
        return source.WordDelimiter(TokenFilterWordDelimiter, wd =>
                                        wd.GenerateWordParts()
                                          .CatenateWords()
                                          .PreserveOriginal(false)
        );
    }


    private static NormalizersDescriptor AddVerenigingZoekNormalizer(NormalizersDescriptor ad)
        => ad.Custom(VerenigingZoekDocumentMapping.BeheerZoekenNormalizer,
                     selector: ca
                         => ca
                           .CharFilters(CharFilterUnderscoreReplace, CharFilterDotReplace)
                           .Filters("lowercase", "asciifolding", "trim")
        );

    private static AnalyzersDescriptor AddVerenigingZoekAnalyzer(AnalyzersDescriptor ad)
        => ad.Custom(VerenigingZoekDocumentMapping.BeheerZoekenAnalyzer,
                     selector: ca
                         => ca
                           .Tokenizer("standard")
                           .CharFilters("underscore_replace", "dot_replace")
                           .Filters("lowercase", "asciifolding", "dutch_stop")
        );
}
