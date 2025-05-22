namespace AssociationRegistry.Admin.Api.Queries;

using Events;
using Framework;
using Marten;

public interface IVerenigingenWithoutGeotagsQuery : IQuery<string[]>;

public class VerenigingenWithoutGeotagsQuery : IVerenigingenWithoutGeotagsQuery
{
    private readonly IDocumentSession _session;
    private readonly ILogger<VerenigingenWithoutGeotagsQuery> _logger;

    public VerenigingenWithoutGeotagsQuery(IDocumentSession session, ILogger<VerenigingenWithoutGeotagsQuery> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<string[]> ExecuteAsync(CancellationToken cancellationToken)
    {
        var vzers = await _session.Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                 .Select(x => x.VCode)
                                 .ToListAsync(cancellationToken);

        var kboVerenigingen = await _session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                           .Select(x => x.VCode)
                                           .ToListAsync(cancellationToken);

        var gemigreerdeVzers = await _session
                                    .Events
                                    .QueryRawEventDataOnly<
                                         FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>()
                                    .Select(x => x.VCode)
                                    .ToListAsync(cancellationToken);

        var geotagsWerdenBepaald = await _session.Events.QueryRawEventDataOnly<GeotagsWerdenBepaald>().Select(x => x.VCode)
                                                .ToListAsync(cancellationToken);
        _logger.LogInformation(
            "Found {VZERS} VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd. " +
            "Found {KBOS} VerenigingMetRechtspersoonlijkheidWerdGeregistreerd. " +
            "Found {GEMIGREERDE} FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid" +
            "Found {GEOTAGS} GeotagsWerdenBepaald",
            vzers.Count(), kboVerenigingen.Count, gemigreerdeVzers.Count, geotagsWerdenBepaald.Count);

        return vzers.Concat(kboVerenigingen)
                    .Concat(gemigreerdeVzers)
                    .Except(geotagsWerdenBepaald)
                    .ToArray();
    }
}
