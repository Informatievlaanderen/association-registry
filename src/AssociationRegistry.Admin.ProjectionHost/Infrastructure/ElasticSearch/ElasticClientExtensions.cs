﻿namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;

using Schema.Search;
using Extensions;
using Nest;
using Nest.Specification.IndicesApi;

public static class ElasticClientExtensions
{
    public static CreateIndexResponse CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => indicesNamespace.Create(
            index,
            selector: descriptor =>
                descriptor
                   .Settings(s => s
                                .Analysis(a => a.CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                     selector: prcf
                                                                                         => prcf.Pattern("\\.").Replacement(""))
                                                                     .PatternReplace(name: "underscore_replace",
                                                                                     selector: prcf
                                                                                         => prcf.Pattern("_").Replacement(" ")))
                                                .TokenFilters(AddDutchStopWordsFilter)
                                                .Normalizers(AddVerenigingZoekNormalizer)))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

    public static async Task<CreateIndexResponse> CreateVerenigingIndexAsync(this IndicesNamespace indicesNamespace, IndexName index)
        => await indicesNamespace.CreateAsync(
            index,
            selector: descriptor =>
                descriptor
                   .Settings(s => s
                                .Analysis(a => a.CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                     selector: prcf
                                                                                         => prcf.Pattern("\\.").Replacement(""))
                                                                     .PatternReplace(name: "underscore_replace",
                                                                                     selector: prcf
                                                                                         => prcf.Pattern("_").Replacement(" ")))
                                                .TokenFilters(AddDutchStopWordsFilter)
                                                .Normalizers(AddVerenigingZoekNormalizer)))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

    public static void CreateDuplicateDetectionIndex(this IndicesNamespace indicesNamespace, IndexName index)
    {
        var createIndexResponse = indicesNamespace.Create(
            index,
            selector: c => c
                          .Settings(s => s
                                       .Analysis(a => a
                                                     .CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                          selector: prcf
                                                                                              => prcf.Pattern("\\.").Replacement(""))
                                                                          .PatternReplace(name: "underscore_replace",
                                                                                          selector: prcf
                                                                                              => prcf.Pattern("_").Replacement(" ")))
                                                     .Analyzers(AddDuplicateDetectionAnalyzer)
                                                     .TokenFilters(AddDutchStopWordsFilter)))
                          .Map<DuplicateDetectionDocument>(DuplicateDetectionDocumentMapping.Get));

        if (!createIndexResponse.IsValid &&
            !createIndexResponse.IndexAlreadyExisted())
            throw createIndexResponse.OriginalException;
    }

    public static async Task<CreateIndexResponse> CreateDuplicateDetectionIndexAsync(
        this IndicesNamespace indicesNamespace,
        IndexName index)
        => await indicesNamespace.CreateAsync(
            index,
            selector: c => c
                          .Settings(s => s
                                       .Analysis(a => a
                                                     .CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                          selector: prcf
                                                                                              => prcf.Pattern("\\.").Replacement(""))
                                                                          .PatternReplace(name: "underscore_replace",
                                                                                          selector: prcf
                                                                                              => prcf.Pattern("_").Replacement(" ")))
                                                     .Analyzers(AddDuplicateDetectionAnalyzer)
                                                     .TokenFilters(AddDutchStopWordsFilter)))
                          .Map<DuplicateDetectionDocument>(DuplicateDetectionDocumentMapping.Get));

    private static TokenFiltersDescriptor AddDutchStopWordsFilter(TokenFiltersDescriptor tf)
        => tf.Stop(name: "dutch_stop", selector: st => st
                      .StopWords("_dutch_") // Or provide your custom list
        );

    private static AnalyzersDescriptor AddDuplicateDetectionAnalyzer(AnalyzersDescriptor ad)
        => ad.Custom(DuplicateDetectionDocumentMapping.DuplicateAnalyzer,
                     selector: ca
                         => ca
                           .Tokenizer("standard")
                           .CharFilters("underscore_replace", "dot_replace")
                           .Filters("lowercase", "asciifolding", "dutch_stop")
        );

    private static NormalizersDescriptor AddVerenigingZoekNormalizer(NormalizersDescriptor ad)
        => ad.Custom(VerenigingZoekDocumentMapping.BeheerZoekenNormalizer,
                     selector: ca
                         => ca
                           .CharFilters("underscore_replace", "dot_replace")
                           .Filters("lowercase", "asciifolding", "trim")
        );

    private static bool IndexAlreadyExisted(this CreateIndexResponse createIndexResponse)
    {
        return createIndexResponse.ServerError.Error.Type == WellknownElasticSearchErrorTypes.ResourceAlreadyExistsException;
    }
}
