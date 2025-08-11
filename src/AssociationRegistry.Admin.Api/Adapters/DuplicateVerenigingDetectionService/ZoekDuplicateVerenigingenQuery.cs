namespace AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;

using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging;
using Schema.Search;
using GemeentenaamVerrijking;
using Vereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using System.Collections.Immutable;
using LogLevel = LogLevel;

public class ZoekDuplicateVerenigingenQuery : IDuplicateVerenigingDetectionService
{
    private readonly IElasticClient _client;
    private readonly MinimumScore _defaultMinimumScore;
    private readonly ILogger<ZoekDuplicateVerenigingenQuery> _logger;

    public ZoekDuplicateVerenigingenQuery(
        IElasticClient client,
        MinimumScore defaultMinimumScore,
        ILogger<ZoekDuplicateVerenigingenQuery> logger = null)
    {
        _client = client;
        _defaultMinimumScore = defaultMinimumScore;
        _logger = logger ?? NullLogger<ZoekDuplicateVerenigingenQuery>.Instance;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        Locatie[] locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null)
    {
        return await ExecuteAsync(naam, new DuplicateVerenigingZoekQueryLocaties(locaties), includeScore, minimumScoreOverride);
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        DuplicateVerenigingZoekQueryLocaties locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null)
    {
        minimumScoreOverride ??= _defaultMinimumScore;

        var naamZonderGemeentes = RemoveGemeentenaamFromVerenigingsNaam.Remove(naam, locaties.VerrijkteGemeentes);

        return await Search(naam, includeScore, minimumScoreOverride, naamZonderGemeentes, locaties.Gemeentes, locaties.Postcodes);
    }

    private async Task<IReadOnlyCollection<DuplicaatVereniging>> Search(
        VerenigingsNaam naam,
        bool includeScore,
        MinimumScore minimumScoreOverride,
        string naamZonderGemeentes,
        string[] gemeentes,
        string[] postcodes)
    {
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
            if (searchResponse.OriginalException is not null)
            {
                throw new ElasticSearchException(searchResponse.OriginalException, searchResponse.DebugInformation);
            }
            else
            {
                throw new ElasticSearchException(searchResponse.DebugInformation);
            }

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
                                            .Field(f => f.Locaties
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
                                        DuplicateDetectionDocument>, QueryContainer>(qc => qc
                                                                                        .Nested(n => n
                                                                                                    .Path(p => p.Locaties)
                                                                                                    .Query(nq => nq
                                                                                                                .Match(m => m
                                                                                                                            .Field(f => f
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
                    Naam = VerenigingssubtypeCode.GetNameOrDefaultOrNull(document.Source.VerenigingssubtypeCode!),
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

public class ElasticSearchException : Exception
{
    public ElasticSearchException(Exception searchResponseOriginalException, string debugInformation)
    : base($"{searchResponseOriginalException}\n\nDebug Information: {debugInformation}", searchResponseOriginalException)
    {
    }

    public ElasticSearchException(string debugInformation)
    :base($"Search for duplicate verenigingen failed: {debugInformation}")
    {

    }
}
