namespace AssociationRegistry.Admin.Api.Queries;

using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;

public interface INietKboVerenigingenVCodesQuery : IQuery<VCode[]>;

public class NietKboVerenigingenVCodesQuery : INietKboVerenigingenVCodesQuery
{
    private readonly IDocumentSession _session;
    private readonly ILogger<NietKboVerenigingenVCodesQuery> _logger;

    public NietKboVerenigingenVCodesQuery(IDocumentSession session, ILogger<NietKboVerenigingenVCodesQuery> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<VCode[]> ExecuteAsync(CancellationToken cancellationToken)
    {
        var feitelijkeVerenigingens =
            await _session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens>()
                          .Select(x => x.VCode)
                          .ToListAsync(token: cancellationToken);

        var vzers =
            await _session.Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>()
                          .Select(x => x.VCode)
                          .ToListAsync(token: cancellationToken);

        return feitelijkeVerenigingens
              .Concat(vzers)
              .Select(VCode.Hydrate)
              .ToArray();
    }
}
