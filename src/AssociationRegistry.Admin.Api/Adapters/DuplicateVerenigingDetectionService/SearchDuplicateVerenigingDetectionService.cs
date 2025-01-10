namespace AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;

using Schema.Search;
using DuplicateVerenigingDetection;
using Vereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using System.Collections.Immutable;

public class SearchDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    private readonly IElasticClient _client;
    private readonly ILogger<SearchDuplicateVerenigingDetectionService> _logger;

    public SearchDuplicateVerenigingDetectionService(IElasticClient client, ILogger<SearchDuplicateVerenigingDetectionService> logger = null)
    {
        _client = client;
        _logger = logger ?? NullLogger<SearchDuplicateVerenigingDetectionService>.Instance;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties,
                                                                              bool includeScore = false, MinimumScore? minimumScoreOverride = null)
    {
        minimumScoreOverride ??= MinimumScore.Default;

        var locatiesMetAdres = locaties.Where(l => l.Adres is not null).ToArray();

        if (locatiesMetAdres.Length == 0) return Array.Empty<DuplicaatVereniging>();

        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente.Naam).ToArray();

        var searchResponse =
            await _client
               .SearchAsync<DuplicateDetectionDocument>(
                    s => s
                        .Explain(includeScore)
                        .TrackScores(includeScore)
                         //.MinScore(minimumScoreOverride.Value)
                        .Query(
                             q => q.Bool(
                                 b => b
                                     .Should(
                                          // Original must query
                                          s1 => s1.Bool(

                                              b => b.Must(
                                                  MatchOpNaam(naam)
                                              )),
                                          s2 => s2.Bool(b => b.Must(m => m.Match(ma => ma
                                                                                      .Field(f => f.Naam)
                                                                                      .Query(naam)
                                                                                      .Analyzer(DuplicateDetectionDocumentMapping
                                                                                              .DuplicateFullNameAnalyzer)
                                                                                      .Fuzziness(Fuzziness.Auto)
                                                                                      .Boost(0.5)
                                                                                      .MinimumShouldMatch(2)
                                                                    )
                                                        )
                                              ))
                                                  .MinimumShouldMatch(1) // At least one of the clauses must match
                                                  .Filter(MatchOpPostcodeOfGemeente(gemeentes, postcodes),
                                                          IsNietGestopt,
                                                          IsNietDubbel,
                                                          IsNietVerwijderd)

                             )
                         ));
        _logger.LogInformation("Score for query: {Score}", string.Join(", ", searchResponse.Hits.Select(x => $"{x.Score} {x.Source.Naam}")));
        searchResponse.Hits.ToList().ForEach(x =>
        {
            _logger.LogInformation("Query: {Query}Explanation for Score {Score} of '{Naam}': {@Explanation}", naam, x.Score, x.Source.Naam, x.Explanation);
        });

        return searchResponse.Hits
                             .Select(ToDuplicateVereniging)
                             .ToArray();
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchOpPostcodeOfGemeente(
        string[] gemeentes,
        string[] postcodes)
    {
        return f =>
            f.Bool(fb =>
                       fb
                          .Should( // Use should within a filter context for municipalities
                               MatchOpGemeente(gemeentes)
                                  .Append(
                                       MatchOpPostcode(postcodes))
                           )
                          .MinimumShouldMatch(
                               1) // At least one of the location conditions must match
            );
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
                      .Field(f => f.Naam)
                      .Query(naam)
                      .Analyzer(DuplicateDetectionDocumentMapping.DuplicateAnalyzer)
                      .Fuzziness(Fuzziness.AutoLength(3, 3))); // Assumes this analyzer applies lowercase and asciifolding
        //.MinimumShouldMatch("90%") // You can adjust this percentage as needed);
    }

    private static DuplicaatVereniging ToDuplicateVereniging(IHit<DuplicateDetectionDocument> document)
        => new(
            document.Source.VCode,
            new DuplicaatVereniging.VerenigingsType(document.Source.VerenigingsTypeCode,
                                                    Verenigingstype.Parse(document.Source.VerenigingsTypeCode).Naam),
            document.Source.Naam,
            document.Source.KorteNaam,
            document.Source.HoofdactiviteitVerenigingsloket?
                    .Select(h => new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(
                                h, HoofdactiviteitVerenigingsloket.Create(h).Naam)).ToImmutableArray() ?? [],
            document.Source.Locaties.Select(ToLocatie).ToImmutableArray(),
            IncludesScore(document)
                ? new DuplicaatVereniging.ScoringInfo(document.Explanation.Description, document.Score)
                : DuplicaatVereniging.ScoringInfo.NotApplicable
        );

    private static bool IncludesScore(IHit<DuplicateDetectionDocument> document)
        => document.Explanation is not null && document.Score is not null;

    private static DuplicaatVereniging.Locatie ToLocatie(DuplicateDetectionDocument.Locatie loc)
        => new(
            loc.Locatietype,
            loc.IsPrimair,
            loc.Adresvoorstelling,
            loc.Naam,
            loc.Postcode,
            loc.Gemeente);
}
