namespace AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;

using Schema.Search;
using DuplicateVerenigingDetection;
using GemeentenaamDecorator;
using Vereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using System.Collections.Immutable;
using LogLevel = LogLevel;

public class SearchDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    private readonly IElasticClient _client;
    private readonly MinimumScore _defaultMinimumScore;
    private readonly ILogger<SearchDuplicateVerenigingDetectionService> _logger;

    public SearchDuplicateVerenigingDetectionService(
        IElasticClient client,
        MinimumScore defaultMinimumScore,
        ILogger<SearchDuplicateVerenigingDetectionService> logger = null)
    {
        _client = client;
        _defaultMinimumScore = defaultMinimumScore;
        _logger = logger ?? NullLogger<SearchDuplicateVerenigingDetectionService>.Instance;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(
        VerenigingsNaam naam,
        Locatie[] locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null)
    {
        minimumScoreOverride ??= _defaultMinimumScore;

        var locatiesMetAdres = locaties.Where(l => l.Adres is not null).ToArray();

        if (locatiesMetAdres.Length == 0) return Array.Empty<DuplicaatVereniging>();

        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente.Naam).ToArray();

        var verrijkteGemeentes = gemeentes.Select(x => VerrijkteGemeentenaam.FromGemeentenaam(new Gemeentenaam(x))).ToArray();
        var naamZonderGemeentes = RemoveGemeentenaamFromVerenigingsNaam.Remove(naam, verrijkteGemeentes);

        var searchResponse =
            await _client
               .SearchAsync<DuplicateDetectionDocument>(s =>
                                                            s
                                                               .Explain(includeScore)
                                                               .TrackScores(includeScore)
                                                               .MinScore(minimumScoreOverride.Value)
                                                               .Query(p => p.Bool(b =>
                                                                                      b.Should(
                                                                                            MatchOpNaam(
                                                                                                VerenigingsNaam.Create(
                                                                                                    naamZonderGemeentes)),
                                                                                            MatchOpFullNaam(
                                                                                                VerenigingsNaam.Create(
                                                                                                    naamZonderGemeentes)))
                                                                                       .MinimumShouldMatch(1)
                                                                                       .Filter(
                                                                                            MatchOpPostcodeOfGemeente(
                                                                                                gemeentes, postcodes),
                                                                                            IsNietDubbel,
                                                                                            IsNietGestopt,
                                                                                            IsNietVerwijderd)
                                                                      )));

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogScoreAndExplanation(naam, searchResponse);
        }

        if (!searchResponse.IsValid)
            throw searchResponse.OriginalException;

        return searchResponse.Hits
                             .Select(ToDuplicateVereniging)
                             .ToArray();
    }

    private void LogScoreAndExplanation(VerenigingsNaam naam, ISearchResponse<DuplicateDetectionDocument> searchResponse)
    {
        _logger.LogDebug("Score for query: {Score}",
                         string.Join(", ", searchResponse.Hits.Select(x => $"{x.Score} {x.Source.Naam}")));

        searchResponse.Hits.ToList().ForEach(x =>
        {
            _logger.LogDebug("Query: {Query}Explanation for Score {Score} of '{Naam}': {@Explanation}", naam, x.Score, x.Source.Naam,
                             x.Explanation);
        });
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchOpPostcodeOfGemeente(
        string[] gemeentes,
        string[] postcodes)
    {
        return f =>
            f.Bool(fb =>
                       fb
                          .Should(MatchOpGemeente(gemeentes)
                                     .Append(MatchOpPostcode(postcodes))
                           )
                          .MinimumShouldMatch(
                               1));
    }

    private static QueryContainer IsNietGestopt(QueryContainerDescriptor<DuplicateDetectionDocument> descriptor)
    {
        return descriptor.Term(queryDescriptor => queryDescriptor.Field(document => document.IsGestopt)
                                                                 .Value(false));
    }

    private static QueryContainer IsNietDubbel(QueryContainerDescriptor<DuplicateDetectionDocument> descriptor)
    {
        return descriptor.Term(queryDescriptor => queryDescriptor.Field(document => document.IsDubbel)
                                                                 .Value(false));
    }

    private static QueryContainer IsNietVerwijderd(QueryContainerDescriptor<DuplicateDetectionDocument> shouldDescriptor)
    {
        return shouldDescriptor
           .Term(termDescriptor
                     => termDescriptor
                       .Field(document => document.IsVerwijderd)
                       .Value(false));
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchOpPostcode(string[] postcodes)
    {
        return postalCodesQuery => postalCodesQuery
           .Nested(n => n
                       .Path(p => p.Locaties)
                       .Query(nq => nq
                                 .Terms(t => t
                                            .Field(
                                                 f => f.Locaties
                                                       .First()
                                                       .Postcode)
                                            .Terms(postcodes)
                                  )
                        )
            );
    }

    private static IEnumerable<Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer>> MatchOpGemeente(
        string[] gemeentes)
    {
        return gemeentes.Select(gemeente =>
                                    new Func<QueryContainerDescriptor<
                                        DuplicateDetectionDocument>, QueryContainer>(
                                        qc => qc
                                           .Nested(n => n
                                                       .Path(p => p.Locaties)
                                                       .Query(nq => nq
                                                                 .Match(m => m
                                                                            .Field(
                                                                                 f => f
                                                                                     .Locaties
                                                                                     .First()
                                                                                     .Gemeente)
                                                                            .Query(
                                                                                 gemeente)
                                                                  )
                                                        )
                                            )
                                    )
        );
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchOpNaam(VerenigingsNaam naam)
    {
        return must => must
           .Match(m => m
                      .Field(f => f.Naam.Suffix("naam"))
                      .Query(naam)
                      .Analyzer(DuplicateDetectionDocumentMapping.DuplicateAnalyzer)
                      .MinimumShouldMatch("67%"));
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchOpFullNaam(VerenigingsNaam naam)
    {
        return must => must
           .Match(m => m
                      .Field(f => f.Naam.Suffix("naamexact"))
                      .Query(naam)
                      .Analyzer(DuplicateDetectionDocumentMapping.DuplicateFullNameAnalyzer)
                      .Boost(2)
                      .Fuzziness(
                           Fuzziness.Auto)
                      .MinimumShouldMatch("67%"));
    }

    private static DuplicaatVereniging ToDuplicateVereniging(IHit<DuplicateDetectionDocument> document)
        => new(
            document.Source.VCode,
            new DuplicaatVereniging.Types.Verenigingstype()
            {
                Code = document.Source.VerenigingsTypeCode,
                Naam = Verenigingstype.Parse(document.Source.VerenigingsTypeCode).Naam,
            },
            Verenigingstype.IsKboVereniging(document.Source.VerenigingsTypeCode)
                ? null
                : new DuplicaatVereniging.Types.Verenigingssubtype()
                {
                    Code = document.Source.VerenigingssubtypeCode!,
                    Naam = Verenigingssubtype.GetNameOrDefaultOrNull(document.Source.VerenigingssubtypeCode!),
                },
            document.Source.Naam,
            document.Source.KorteNaam,
            document.Source.HoofdactiviteitVerenigingsloket?
               .Select(h => new DuplicaatVereniging.Types.HoofdactiviteitVerenigingsloket(
                           h, HoofdactiviteitVerenigingsloket.Create(h).Naam)).ToImmutableArray() ?? [],
            [..document.Source.Locaties.Select(ToLocatie)],
            IncludesScore(document)
                ? new DuplicaatVereniging.Types.ScoringInfo(document.Explanation.Description, document.Score)
                : DuplicaatVereniging.Types.ScoringInfo.NotApplicable
        );

    private static bool IncludesScore(IHit<DuplicateDetectionDocument> document)
        => document.Explanation is not null && document.Score is not null;

    private static DuplicaatVereniging.Types.Locatie ToLocatie(DuplicateDetectionDocument.Locatie loc)
        => new(
            loc.Locatietype,
            loc.IsPrimair,
            loc.Adresvoorstelling,
            loc.Naam,
            loc.Postcode,
            loc.Gemeente);
}
