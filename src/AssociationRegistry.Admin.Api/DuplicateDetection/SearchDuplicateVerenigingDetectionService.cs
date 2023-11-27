namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using DuplicateVerenigingDetection;
using Nest;
using Schema.Search;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vereniging;

public class SearchDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    private readonly IElasticClient _client;

    public SearchDuplicateVerenigingDetectionService(IElasticClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties)
    {
        var locatiesMetAdres = locaties.Where(l => l.Adres is not null).ToArray();

        if (locatiesMetAdres.Length == 0) return Array.Empty<DuplicaatVereniging>();

        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente).ToArray();

        var searchResponse = await _client.SearchAsync<DuplicateDetectionDocument>(
            s => s
               .Query(q => q
                         .Bool(b => b
                                  .Should(
                                       sh => sh.Bool(sb => sb
                                                        .Must(
                                                             mu => MatchNaam(mu, naam),
                                                             mu => MatchGemeente(mu, gemeentes)
                                                         )
                                       ),
                                       sh => sh.Bool(sb => sb
                                                        .Must(
                                                             mu => MatchNaam(mu, naam),
                                                             mu => MatchPostcode(mu, postcodes)
                                                         )
                                       )
                                   )
                          )
                )
        );

        return searchResponse.Documents.Select(ToDuplicateVereniging)
                             .ToArray();
    }

    private static QueryContainer MatchNaam(QueryContainerDescriptor<DuplicateDetectionDocument> mu, VerenigingsNaam naam)
    {
        return mu
           .Match(
                m => m
                    .Field(
                         f => f
                            .Naam)
                    .Query(
                         naam)
                    .Analyzer(
                         DuplicateDetectionDocumentMapping
                            .DuplicateAnalyzer)
                    .Fuzziness(
                         Fuzziness
                            .Auto));
    }

    private static QueryContainer MatchGemeente(QueryContainerDescriptor<DuplicateDetectionDocument> mu, string[] gemeentes)
    {
        return mu
           .Nested(
                n => n
                    .Path(
                         p => p
                            .Locaties)
                    .Query(
                         nq
                             => nq
                                .Match(
                                     m =>
                                         FuzzyMatchOpNaam(
                                             m,
                                             path
                                             : f
                                                 => f
                                                   .Locaties
                                                   .First()
                                                   .Gemeente,
                                             string
                                                .Join(
                                                     separator
                                                     : " ",
                                                     gemeentes))
                                 )
                     )
            );
    }

    private static QueryContainer MatchPostcode(QueryContainerDescriptor<DuplicateDetectionDocument> mu, string[] postcodes)
    {
        return mu
           .Nested(
                n => n
                    .Path(
                         p => p
                            .Locaties)
                    .Query(
                         nq
                             => nq
                                .Terms(
                                     t => t
                                         .Field(
                                              f => f
                                                  .Locaties
                                                  .First()
                                                  .Postcode)
                                         .Terms(
                                              postcodes)
                                 )
                     )
            );
    }

    private static MatchQueryDescriptor<DuplicateDetectionDocument> FuzzyMatchOpNaam(
        MatchQueryDescriptor<DuplicateDetectionDocument> m,
        Expression<Func<DuplicateDetectionDocument, string>> path,
        string query)
        => m
          .Field(path)
          .Query(query)
          .Analyzer(DuplicateDetectionDocumentMapping
                       .DuplicateAnalyzer)
          .Fuzziness(Fuzziness.Auto) // Assumes this analyzer applies lowercase and asciifolding
          .MinimumShouldMatch("90%");

    private static DuplicaatVereniging ToDuplicateVereniging(DuplicateDetectionDocument document)
        => new(
            document.VCode,
            new DuplicaatVereniging.VerenigingsType(document.VerenigingsTypeCode,
                                                    Verenigingstype.Parse(document.VerenigingsTypeCode).Beschrijving),
            document.Naam,
            document.KorteNaam,
            document.HoofdactiviteitVerenigingsloket
                    .Select(h => new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(
                                h, HoofdactiviteitVerenigingsloket.Create(h).Naam)).ToImmutableArray(),
            document.Locaties.Select(ToLocatie).ToImmutableArray());

    private static DuplicaatVereniging.Locatie ToLocatie(DuplicateDetectionDocument.Locatie loc)
        => new(
            loc.Locatietype,
            loc.IsPrimair,
            loc.Adresvoorstelling,
            loc.Naam,
            loc.Postcode ?? string.Empty,
            loc.Gemeente ?? string.Empty);
}
