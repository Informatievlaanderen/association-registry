namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;

using Nest;
using Nest.Specification.IndicesApi;
using Schema.Search;

public static class ElasticClientExtensions
{
    public static CreateIndexResponse CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
    {
        var createIndexResponse = indicesNamespace.Create(
            index,
            selector: descriptor =>
                descriptor
                   .Settings(s => s
                                .Analysis(a => a
                                              .CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                   selector: prcf
                                                                                       => prcf.Pattern("\\.").Replacement(""))
                                                                   .PatternReplace(name: "underscore_replace",
                                                                                   selector: prcf
                                                                                       => prcf.Pattern("_").Replacement(" ")))
                                              .Normalizers(AddVerenigingZoekNormalizer)
                                 ))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

        if (!createIndexResponse.IsValid)
            throw createIndexResponse.OriginalException;

        return createIndexResponse;
    }

    public static async Task<CreateIndexResponse> CreateVerenigingIndexAsync(this IndicesNamespace indicesNamespace, IndexName index)
    {
        var createIndexResponse = await indicesNamespace.CreateAsync(
            index,
            selector: descriptor =>
                descriptor
                   .Settings(s => s
                                .Analysis(a => a
                                              .CharFilters(cf => cf.PatternReplace(name: "dot_replace",
                                                                                   selector: prcf
                                                                                       => prcf.Pattern("\\.").Replacement(""))
                                                                   .PatternReplace(name: "underscore_replace",
                                                                                   selector: prcf
                                                                                       => prcf.Pattern("_").Replacement(" ")))
                                              .Normalizers(AddVerenigingZoekNormalizer)
                                 ))
                   .Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

        if (!createIndexResponse.IsValid)
            throw createIndexResponse.OriginalException;

        return createIndexResponse;
    }

    private static NormalizersDescriptor AddVerenigingZoekNormalizer(NormalizersDescriptor ad)
        => ad.Custom(VerenigingZoekDocumentMapping.PubliekZoekenAnalyzer,
                     selector: ca
                         => ca
                           .CharFilters("underscore_replace", "dot_replace")
                           .Filters("lowercase", "asciifolding")
        );
}
