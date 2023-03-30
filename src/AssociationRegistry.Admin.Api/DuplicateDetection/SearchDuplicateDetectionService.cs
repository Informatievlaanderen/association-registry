﻿namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Locaties;
using Marten;
using Projections.Detail;
using Vereniging.DuplicateDetection;
using VerenigingsNamen;

public class SearchDuplicateDetectionService : IDuplicateDetectionService
{
    private readonly IQuerySession _session;

    public SearchDuplicateDetectionService(IQuerySession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, LocatieLijst locaties)
    {
        var postcodes = locaties.Select(l => l.Postcode).ToArray();
        var gemeentes = locaties.Select(l => l.Gemeente).ToArray();
        return (await _session.Query<BeheerVerenigingDetailDocument>()
                .Where(
                    document =>
                        document.Naam.Equals(naam, StringComparison.InvariantCultureIgnoreCase) &&
                        document.Locaties.Any(
                            locatie =>
                                locatie.Postcode.IsOneOf(postcodes) ||
                                locatie.Gemeente.IsOneOf(gemeentes)
                        )
                )
                .ToListAsync())
            .Select(ToDuplicateVereniging)
            .ToArray();
    }

    private static DuplicaatVereniging ToDuplicateVereniging(BeheerVerenigingDetailDocument document)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam ?? string.Empty,
            document.HoofdactiviteitenVerenigingsloket.Select(h => new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)).ToImmutableArray(),
            string.Empty,
            document.Locaties.Select(ToLocatie).ToImmutableArray(),
            ImmutableArray<DuplicaatVereniging.Activiteit>.Empty);

    private static DuplicaatVereniging.Locatie ToLocatie(BeheerVerenigingDetailDocument.Locatie loc)
        => new(loc.Locatietype, loc.Hoofdlocatie, loc.Adres, loc.Naam, loc.Postcode, loc.Gemeente);
}
