namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System;
using System.Collections.Immutable;
using System.Linq;
using Constants;
using Nest;

public class SearchVerenigingenMapper
{
    private readonly AppSettings _appSettings;


    public SearchVerenigingenMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ISearchResponse<VerenigingDocument> searchResponse,
        PaginationQueryParams paginationRequest)
        => new()
        {
            Verenigingen = GetVerenigingenFromResponse(_appSettings, searchResponse),
            Facets = new Facets
            {
                HoofdActiviteiten = GetHoofdActiviteitFacets(searchResponse),
            },
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private static Metadata GetMetadata(ISearchResponse<VerenigingDocument> searchResponse, PaginationQueryParams paginationRequest)
        => new(
            new Pagination(
                searchResponse.Total,
                paginationRequest.Offset,
                paginationRequest.Limit
            )
        );

    private static ImmutableArray<HoofdActiviteitFacetItem> GetHoofdActiviteitFacets(ISearchResponse<VerenigingDocument> searchResponse)
        => searchResponse.Aggregations
            .Terms(WellknownFacets.HoofdactiviteitenCountAggregateName)
            .Buckets
            .Select(CreateHoofdActiviteitFacetItem)
            .ToImmutableArray();

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(KeyedBucket<string> bucket)
        => CreateHoofdActiviteitFacetItem(Activiteiten.Hoofdactiviteit.Create(bucket.Key), bucket.DocCount ?? 0);

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(Activiteiten.Hoofdactiviteit hoofdActiviteit, long aantal)
        => new(hoofdActiviteit.Code, hoofdActiviteit.Naam, aantal);

    private static ImmutableArray<Vereniging> GetVerenigingenFromResponse(AppSettings appSettings, ISearchResponse<VerenigingDocument> searchResponse)
        => searchResponse.Hits.Select(
            x =>
                new Vereniging(
                    x.Source.VCode,
                    x.Source.Naam,
                    x.Source.KorteNaam ?? string.Empty,
                    x.Source.Hoofdactiviteiten.Select(h => new Hoofdactiviteit(h.Code, h.Naam)).ToImmutableArray(),
                    new Locatie(string.Empty, string.Empty, x.Source.Hoofdlocatie.Postcode, x.Source.Hoofdlocatie.Gemeente),
                    x.Source.Doelgroep,
                    x.Source.Locaties.Select(locatie => new Locatie(string.Empty, string.Empty, locatie.Postcode, locatie.Gemeente)).ToImmutableArray(),
                    x.Source.Activiteiten.Select(activiteit => new Activiteit(-1, activiteit)).ToImmutableArray(),
                    new VerenigingLinks(new Uri($"{appSettings.BaseUrl}v1/verenigingen/{x.Source.VCode}"))
                )).ToImmutableArray();
}
