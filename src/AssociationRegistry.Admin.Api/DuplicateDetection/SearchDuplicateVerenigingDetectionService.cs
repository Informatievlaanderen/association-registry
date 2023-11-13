namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using DuplicateVerenigingDetection;
using Nest;
using Schema.Search;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        var propereNaam = naam
                         .ToString()
                         .Trim()
                         .Normalize();



        var searchResponse =
            await _client
               .SearchAsync<DuplicateDetectionDocument>(
                    s => s
                       .Query(
                            q => q.Bool(
                                b => b.Must(must => must
                                               .Match(m => m
                                                          .Field(f => f.Naam)
                                                          .Query(naam)
                                                          .Analyzer(DuplicateDetectionDocumentMapping
                                                                       .DuplicateAnalyzer)
                                                          .Fuzziness(
                                                               Fuzziness
                                                                  .Auto) // Assumes this analyzer applies lowercase and asciifolding
                                                          .MinimumShouldMatch("90%") // You can adjust this percentage as needed
                                                )))));
                                      // .Filter(f => f
                                      //                        .Bool(fb => fb
                                      //                                   .Should( // Use should within a filter context for municipalities and postal codes
                                      //                                        gemeentesQuery => gemeentesQuery
                                      //                                           .Nested(n => n
                                      //                                                      .Path(p => p.Locaties)
                                      //                                                      .Query(nq => nq
                                      //                                                                  .Match(m => m
                                      //                                                                              .Field(f => f.Locaties
                                      //                                                                                      .First()
                                      //                                                                                      .Gemeente)
                                      //                                                                              .Query(
                                      //                                                                                   string.Join(
                                      //                                                                                       separator: " ",
                                      //                                                                                       gemeentes))
                                      //                                                                              .Fuzziness(
                                      //                                                                                   Fuzziness.Auto)
                                      //                                                                              .Analyzer(
                                      //                                                                                   DuplicateDetectionDocumentMapping
                                      //                                                                                      .DuplicateAnalyzer)
                                      //                                                                   )
                                      //                                                       )
                                      //                                            ),
                                      //                                        postalCodesQuery => postalCodesQuery
                                      //                                           .Nested(n => n
                                      //                                                      .Path(p => p.Locaties)
                                      //                                                      .Query(nq => nq
                                      //                                                                  .Terms(t => t
                                      //                                                                              .Field(f => f.Locaties
                                      //                                                                                      .First()
                                      //                                                                                      .Postcode)
                                      //                                                                              .Terms(postcodes)
                                      //                                                                   )
                                      //                                                       )
                                      //                                            )
                                      //                                    )
                                      //                                   .MinimumShouldMatch(
                                      //                                        1) // At least one of the location conditions must match
                                      //                         )
                //                 )
                //             )
                //         )
                // );

        return searchResponse.Documents.Select(ToDuplicateVereniging)
                             .ToArray();
    }

    private static DuplicaatVereniging ToDuplicateVereniging(DuplicateDetectionDocument document)
        => new(
            document.VCode,
            new DuplicaatVereniging.VerenigingsType(string.Empty, string.Empty),
            document.Naam,
            string.Empty,
            ImmutableArray<DuplicaatVereniging.HoofdactiviteitVerenigingsloket>.Empty,
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
