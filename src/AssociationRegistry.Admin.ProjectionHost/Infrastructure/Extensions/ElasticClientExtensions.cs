namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Nest;
using Nest.Specification.IndicesApi;
using Schema.Search;

public static class ElasticClientExtensions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => indicesNamespace.Create(
            index,
            selector: descriptor =>
                descriptor.Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

    public static void CreateDuplicateDetectionIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => indicesNamespace.Create(
            index,
            selector: c => c
                          .Settings(s => s
                                       .Analysis(a => a
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
                           .Tokenizer("lowercase")
                           .Filters("asciifolding", "dutch_stop")
        );
}
