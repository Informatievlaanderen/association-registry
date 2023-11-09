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
                                                     .Analyzers(ad => ad
                                                                   .Custom(DuplicateDetectionDocumentMapping.DuplicateAnalyzer, selector: ca
                                                                               => ca
                                                                                 .Tokenizer("standard")
                                                                                 .Filters("lowercase", "asciifolding", "dutch_stop")
                                                                    ))
                                                     .TokenFilters(tf => tf
                                                                      .Stop(name: "dutch_stop", selector: st => st
                                                                               .StopWords("_dutch_") // Or provide your custom list
                                                                       )
                                                      )
                                        ))
                          .Map<DuplicateDetectionDocument>(DuplicateDetectionDocumentMapping.Get));
}
