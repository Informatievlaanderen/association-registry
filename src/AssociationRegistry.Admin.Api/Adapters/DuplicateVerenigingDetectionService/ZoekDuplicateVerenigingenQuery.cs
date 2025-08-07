namespace AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;

using Schema.Search;
using DuplicateVerenigingDetection;
using GemeentenaamVerrijking;
using Vereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Middleware;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Hosts.Configuration.ConfigurationBindings;
using System.Collections.Immutable;
using LogLevel = LogLevel;

public class ZoekDuplicateVerenigingenQuery : IDuplicateVerenigingDetectionService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSearchOptionsSection _elasticSearchOptionsSection;
    private readonly MinimumScore _defaultMinimumScore;
    private readonly ILogger<ZoekDuplicateVerenigingenQuery> _logger;

    public ZoekDuplicateVerenigingenQuery(
        ElasticsearchClient client,
        ElasticSearchOptionsSection elasticSearchOptionsSection,
        MinimumScore defaultMinimumScore,
        ILogger<ZoekDuplicateVerenigingenQuery> logger = null)
    {
        _client = client;
        _elasticSearchOptionsSection = elasticSearchOptionsSection;
        _defaultMinimumScore = defaultMinimumScore;
        _logger = logger ?? NullLogger<ZoekDuplicateVerenigingenQuery>.Instance;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        Locatie[] locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null)
        => await ExecuteAsync(naam, new DuplicateVerenigingZoekQueryLocaties(locaties), includeScore, minimumScoreOverride);

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
        var searchRequest = new SearchRequest<DuplicateDetectionDocument>(_elasticSearchOptionsSection.Indices!.DuplicateDetection!)
        {
            Explain = includeScore,
            TrackScores = includeScore,
            MinScore = minimumScoreOverride.Value,
            Query = new BoolQuery
            {
                Should = new List<Query>
                {
                    MatchOpNaam(naamZonderGemeentes),
                    MatchOpFullNaam(naamZonderGemeentes)
                },
                Filter = new List<Query>
                {
                    MatchOpPostcodeOfGemeente(gemeentes, postcodes),
                    new TermQuery { Field = FieldTerms.IsDubbel, Value = false },
                    new TermQuery { Field = FieldTerms.IsGestopt, Value = false },
                    new TermQuery { Field = FieldTerms.IsVerwijderd, Value = false }
                },
                MinimumShouldMatch = "1"
            }
        };

        try
        {
            var searchResponse = await _client.SearchAsync<DuplicateDetectionDocument>(searchRequest, CancellationToken.None);

            if (!searchResponse.IsValidResponse)
            {
                throw new ElasticSearchException(searchResponse.DebugInformation);
            }

            return searchResponse.Hits
                                 .Select(ToDuplicateVereniging)
                                 .ToArray();
        }
        catch (Exception ex)
        {
            throw new ElasticSearchException(ex, "Search for duplicate verenigingen failed.");
        }
    }

    private void LogScoreAndExplanation(VerenigingsNaam naam, SearchResponse<DuplicateDetectionDocument> searchResponse)
    {
        _logger.LogDebug("Score for query: {Score}",
                         string.Join(", ", searchResponse.Hits.Select(x => $"{x.Score} {x.Source.Naam}")));

        searchResponse.Hits.ToList().ForEach(x =>
        {
            _logger.LogDebug("Query: {Query}Explanation for Score {Score} of '{Naam}': {@Explanation}", naam, x.Score, x.Source.Naam,
                             x.Explanation);
        });
    }

    private static Query MatchOpPostcodeOfGemeente(string[] gemeentes, string[] postcodes)
    {
        var shouldQueries = new List<Query>();

        if (postcodes.Length > 0)
        {
            shouldQueries.Add(new NestedQuery
            {
                Path = "locaties",
                Query = new TermsQuery
                {
                    Field = FieldTerms.Postcode,
                    Terms = new TermsQueryField(postcodes.Select(FieldValue.String).ToList())
                }
            });
        }

        foreach (var gemeente in gemeentes)
        {
            shouldQueries.Add(new NestedQuery
            {
                Path = "locaties",
                Query = new MatchQuery
                {
                    Field = FieldTerms.Gemeente,
                    Query = gemeente
                }
            });
        }

        return new BoolQuery
        {
            Should = shouldQueries,
            MinimumShouldMatch = "1"
        };
    }

    private static Query MatchOpNaam(string naam)
    {
        return new MatchQuery
        {
            Field = FieldTerms.Naam,
            Query = naam,
            Analyzer = DuplicateDetectionDocumentMapping.DuplicateAnalyzer,
            MinimumShouldMatch = "67%"
        };
    }

    private static Query MatchOpFullNaam(string naam)
    {
        return new MatchQuery
        {
            Field = FieldTerms.NaamExact,
            Query = naam,
            Analyzer = DuplicateDetectionDocumentMapping.DuplicateFullNameAnalyzer,
            Boost = 2,
            Fuzziness = "AUTO",
            MinimumShouldMatch = "67%"
        };
    }

    private static DuplicaatVereniging ToDuplicateVereniging(Hit<DuplicateDetectionDocument> document)
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

    private static bool IncludesScore(Hit<DuplicateDetectionDocument> document)
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
