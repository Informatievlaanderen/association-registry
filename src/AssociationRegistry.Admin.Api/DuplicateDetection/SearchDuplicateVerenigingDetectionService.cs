namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using DuplicateVerenigingDetection;
using Nest;
using Schema.Search;
using System;
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

        if (locatiesMetAdres.Length == 0) return Array.Empty<DuplicaatVereniging>();

        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente).ToArray();

        var searchResponse =
            await _client
               .SearchAsync<DuplicateDetectionDocument>(
                    s => s
                       .Query(
                            q => q.Bool(
                                b => b.Must(
                                           MatchOpNaam(naam),
                                           IsNietGestopt
                                       )
                                      .MustNot(BeVerwijderd)
                                      .Filter(MatchOpPostcodeOfGemeente(gemeentes, postcodes)
                                       )
                            )
                        ));

        // TODO : Handle invalid NEST response
        return searchResponse.Documents.Select(ToDuplicateVereniging)
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

    private static QueryContainer BeVerwijderd(QueryContainerDescriptor<DuplicateDetectionDocument> shouldDescriptor)
    {
        return shouldDescriptor
           .Term(termDescriptor
                     => termDescriptor
                       .Field(document => document.IsVerwijderd)
                       .Value(true));
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
                      .Analyzer(DuplicateDetectionDocumentMapping
                                   .DuplicateAnalyzer)
                      .Fuzziness(
                           Fuzziness
                              .Auto) // Assumes this analyzer applies lowercase and asciifolding
                      .MinimumShouldMatch("90%") // You can adjust this percentage as needed
            );
    }

    private static DuplicaatVereniging ToDuplicateVereniging(DuplicateDetectionDocument document)
        => new(
            document.VCode,
            new DuplicaatVereniging.VerenigingsType(document.VerenigingsTypeCode,
                                                    Verenigingstype.Parse(document.VerenigingsTypeCode).Naam),
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
            loc.Postcode,
            loc.Gemeente);
}
