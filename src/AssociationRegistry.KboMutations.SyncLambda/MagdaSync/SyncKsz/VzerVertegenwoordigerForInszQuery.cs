namespace AssociationRegistry.KboMutations.SyncLambda.MagdaSync.SyncKsz;

using DecentraalBeheer.Vereniging;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;
using Queries;

public class VzerVertegenwoordigerForInszQuery
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;
    private readonly IFilterVzerOnlyQuery _filterVzerOnlyQuery;
    private readonly ILogger<VzerVertegenwoordigerForInszQuery> _logger;

    public VzerVertegenwoordigerForInszQuery(
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        IFilterVzerOnlyQuery filterVzerOnlyQuery,
        ILogger<VzerVertegenwoordigerForInszQuery> logger
    )
    {
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
        _logger = logger;
        _filterVzerOnlyQuery = filterVzerOnlyQuery;
    }

    public async Task<List<VertegenwoordigerPersoonsgegevens>> ExecuteAsync(
        Insz insz,
        CancellationToken cancellationToken
    )
    {
        var vertegenwoordigerPersoonsgegevens = await _vertegenwoordigerPersoonsgegevensRepository.Get(
            insz,
            cancellationToken
        );

        if (!vertegenwoordigerPersoonsgegevens.Any())
        {
            // TODO: uitschrijven or-2939

            _logger.LogWarning(
                "Skipping message because this person did not match any known VertegenwoordigerPersoonsgegevensDocument"
            );

            return [];
        }

        var vzerOnly = await FilterOnlyVzer(vertegenwoordigerPersoonsgegevens, cancellationToken);

        if (vzerOnly.Count == 0)
        {
            _logger.LogInformation("Only found kbo associations for this person");
        }

        return vzerOnly;
    }

    private async Task<List<VertegenwoordigerPersoonsgegevens>> FilterOnlyVzer(
        VertegenwoordigerPersoonsgegevens[] vertegenwoordigerPersoonsgegevens,
        CancellationToken cancellationToken
    )
    {
        var vertegenwoordigerPersoonsgegevensByVCode = vertegenwoordigerPersoonsgegevens
            .DistinctBy(x => (x.VCode, x.VertegenwoordigerId))
            .ToList();

        var vzerOnlyVcodes = await _filterVzerOnlyQuery.ExecuteAsync(
            new FilterVzerOnlyQueryFilter(vertegenwoordigerPersoonsgegevensByVCode.Select(x => x.VCode).ToArray()),
            cancellationToken
        );

        var vzerOnly = vertegenwoordigerPersoonsgegevensByVCode.Where(x => vzerOnlyVcodes.Contains(x.VCode)).ToList();

        return vzerOnly;
    }
}
