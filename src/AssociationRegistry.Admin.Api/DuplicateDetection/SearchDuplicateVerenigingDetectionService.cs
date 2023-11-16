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
        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente).ToArray();

        var searchResponse =
            await _client
               .SearchAsync<DuplicateDetectionDocument>(
                    s => s.Query(
                        q => q.Bool(
                            b => b.Must(must => must.Match(m => FuzzyMatchOpNaam(m, f => f.Naam, naam)))
                                  .Filter(f => f.Bool(
                                              fb => fb.Should(MatchGemeente(gemeentes),
                                                              MatchPostcode(postcodes))
                                                      .MinimumShouldMatch(1))))));

        return searchResponse.Documents.Select(ToDuplicateVereniging)
                             .ToArray();
    }

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchPostcode(string[] postcodes)
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

    private static Func<QueryContainerDescriptor<DuplicateDetectionDocument>, QueryContainer> MatchGemeente(string[] gemeentes)
    {
        return gemeentesQuery => gemeentesQuery
           .Nested(n => n
                       .Path(p => p.Locaties)
                       .Query(nq => nq
                                 .Match(m =>
                                            FuzzyMatchOpNaam(m,
                                                             f => f.Locaties
                                                                   .First()
                                                                   .Gemeente, string.Join(
                                                                 separator: " ",
                                                                 gemeentes))
                                  )
                        )
            );
    }

    private static MatchQueryDescriptor<DuplicateDetectionDocument> FuzzyMatchOpNaam(
        MatchQueryDescriptor<DuplicateDetectionDocument> m,
        Expression<Func<DuplicateDetectionDocument, string>> path,
        string query)
    {
        return m
              .Field(path)
              .Query(query)
              .Analyzer(DuplicateDetectionDocumentMapping
                           .DuplicateAnalyzer)
              .Fuzziness(Fuzziness.Auto) // Assumes this analyzer applies lowercase and asciifolding
              .MinimumShouldMatch("90%");
    }

    private static DuplicaatVereniging ToDuplicateVereniging(DuplicateDetectionDocument document)
        => new(
            document.VCode,
            new DuplicaatVereniging.VerenigingsType(document.VerenigingsTypeCode,
                                                    Verenigingstype.Parse(document.VerenigingsTypeCode).Beschrijving),
            document.Naam,
            document.KorteNaam,
            document.HoofdactiviteitVerenigingsloket
                    .Select(h => new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(
                                h, HoofdactiviteitVerenigingsloket.Create(h).Beschrijving)).ToImmutableArray(),
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
