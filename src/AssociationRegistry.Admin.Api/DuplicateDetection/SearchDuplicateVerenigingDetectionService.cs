namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DuplicateVerenigingDetection;
using Marten;
using Schema.Constants;
using Schema.Detail;
using Vereniging;

public class SearchDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    private readonly IQuerySession _session;

    public SearchDuplicateVerenigingDetectionService(IQuerySession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties)
    {
        var locatiesMetAdres = locaties.Where(l => l.Adres is not null).ToArray();
        var postcodes = locatiesMetAdres.Select(l => l.Adres!.Postcode).ToArray();
        var gemeentes = locatiesMetAdres.Select(l => l.Adres!.Gemeente).ToArray();
        return (await _session.Query<BeheerVerenigingDetailDocument>()
                .Where(
                    document =>
                        document.Status.Equals(VerenigingStatus.Actief)&&
                        document.Naam.Equals(naam, StringComparison.InvariantCultureIgnoreCase) &&
                        document.Locaties.Any(
                            locatie =>
                                locatie.Adres != null && (
                                locatie.Adres.Postcode.IsOneOf(postcodes) ||
                                locatie.Adres.Gemeente.IsOneOf(gemeentes))
                        )
                )
                .ToListAsync())
            .Select(ToDuplicateVereniging)
            .ToArray();
    }

    private static DuplicaatVereniging ToDuplicateVereniging(BeheerVerenigingDetailDocument document)
        => new(
            document.VCode,
            new DuplicaatVereniging.VerenigingsType(document.Type.Code, document.Type.Beschrijving),
            document.Naam,
            document.KorteNaam ?? string.Empty,
            document.HoofdactiviteitenVerenigingsloket.Select(h => new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)).ToImmutableArray(),
            document.Locaties.Select(ToLocatie).ToImmutableArray());

    private static DuplicaatVereniging.Locatie ToLocatie(BeheerVerenigingDetailDocument.Locatie loc)
        => new(
            loc.Locatietype,
            loc.IsPrimair,
            loc.Adresvoorstelling,
            loc.Naam,
            loc.Adres?.Postcode ?? string.Empty,
            loc.Adres?.Gemeente ?? string.Empty);
}
