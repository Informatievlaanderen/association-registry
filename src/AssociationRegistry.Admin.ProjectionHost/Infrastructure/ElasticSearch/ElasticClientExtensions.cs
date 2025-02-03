namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;

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
                                                .TokenFilters(tf => tf.Stop(name: "dutch_stop", selector: st => st
                                                                               .StopWords("_dutch_") // Or provide your custom list
                                                              ))
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
                                                .TokenFilters(tf => tf.Stop(name: "dutch_stop", selector: st => st
                                                                               .StopWords("_dutch_") // Or provide your custom list
                                                              )   )
                                                .Normalizers(AddVerenigingZoekNormalizer)))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

    public static void CreateDuplicateDetectionIndex(this IndicesNamespace indicesNamespace, IndexName index)
    {
        CreateDuplicateDetectionIndexAsync(indicesNamespace, index).GetAwaiter().GetResult();
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
                                                     .TokenFilters(tf => tf.Stop(name: "dutch_stop", selector: st => st
                                                                                    .StopWords("_dutch_") // Or provide your custom list
                                                                            ).Stop(name: "muni", selector: st => st
                                                                                        .StopWords("kortrijk", "aarschot") // Or provide your custom list
                                                                                )
                                                                               .WordDelimiter("mwd", wd =>
                                                                                    wd.GenerateWordParts()
                                                                                      .CatenateWords()
                                                                                      .SplitOnCaseChange()
                                                                                      .SplitOnNumerics()
                                                                                      .PreserveOriginal())
                                                                               .Shingle("shingle", sd => sd.MinShingleSize(2)
                                                                                           .MaxShingleSize(2)
                                                                                           .OutputUnigrams(true)
                                                                                           .TokenSeparator(""))
                                                                               .Fingerprint("my_fingerprint_filter", f => f
                                                                                                // .Separator("")
                                                                                               .MaxOutputSize(255)
                                                                                ))))
                          .Map<DuplicateDetectionDocument>(DuplicateDetectionDocumentMapping.Get));

    private static AnalyzersDescriptor AddDuplicateDetectionAnalyzer(AnalyzersDescriptor ad)
        => ad.Custom(DuplicateDetectionDocumentMapping.DuplicateAnalyzer,
                     selector: ca
                         => ca
                           .Tokenizer("whitespace")
                           .CharFilters("underscore_replace", "dot_replace")
                           .Filters("lowercase", "asciifolding", "dutch_stop", "muni", "mwd")
        ).Custom(DuplicateDetectionDocumentMapping.DuplicateFullNameAnalyzer,
                 selector: ca
                     => ca
                       .Tokenizer("standard")
                       .CharFilters("underscore_replace", "dot_replace")
                       .Filters("lowercase", "asciifolding", "dutch_stop", "mwd", "shingle")
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
