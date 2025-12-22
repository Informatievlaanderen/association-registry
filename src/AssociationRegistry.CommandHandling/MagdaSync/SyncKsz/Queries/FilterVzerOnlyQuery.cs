namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz.Queries;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;

public interface IFilterVzerOnlyQuery : IQuery<IReadOnlyCollection<VCode>, FilterVzerOnlyQueryFilter>;
public class FilterVzerOnlyQuery : IFilterVzerOnlyQuery
{
    private readonly IDocumentSession _session;

    public FilterVzerOnlyQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyCollection<VCode>> ExecuteAsync(
        FilterVzerOnlyQueryFilter filter, CancellationToken token)
    {
        var vCodeValues = filter.VCodesToFilter.Select(x => x.Value).ToList();

        var gemigreerde =
            (await _session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>()
                           .Where(geregistreerd => vCodeValues.Contains(geregistreerd.VCode))
                           .ToListAsync(token: token))
           .Select(x => x.VCode)
           .ToList();

        var vzers =
            (await _session.Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>()
                           .Where(geregistreerd => vCodeValues.Contains(geregistreerd.VCode))
                           .ToListAsync(token: token))
           .Select(x => x.VCode)
           .ToList();

        return vzers.Union(gemigreerde).Select(VCode.Hydrate).ToList();
    }
}

public class FilterVzerOnlyQueryFilter
{
    public FilterVzerOnlyQueryFilter(VCode[] vCodesToFilter)
    {

        VCodesToFilter = vCodesToFilter;
    }

    public VCode[] VCodesToFilter { get; }
}
