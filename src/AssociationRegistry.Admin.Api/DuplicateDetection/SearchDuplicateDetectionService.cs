namespace AssociationRegistry.Admin.Api.DuplicateDetection;

using System;
using System.Collections.Generic;
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

    public async Task<IReadOnlyCollection<DuplicateCandidate>> GetDuplicates(VerenigingsNaam naam, LocatieLijst locaties)
    {
        var postcodes = locaties.Select(l => l.Postcode).ToArray();
        var gemeentes = locaties.Select(l => l.Gemeente.ToLower()).ToArray();
        return (await _session.Query<BeheerVerenigingDetailDocument>()
                .Where(
                    document =>
                        document.Naam.Equals(naam, StringComparison.InvariantCultureIgnoreCase) &&
                        document.Locaties.Any(
                            locatie =>
                                locatie.Postcode.IsOneOf(postcodes) ||
                                locatie.Gemeente.ToLower().IsOneOf(gemeentes)
                        )
                )
                .ToListAsync())
            .Select(ToDuplicateCandidate)
            .ToArray();
    }

    private static DuplicateCandidate ToDuplicateCandidate(BeheerVerenigingDetailDocument document)
        => new(document.VCode, document.Naam);
}
