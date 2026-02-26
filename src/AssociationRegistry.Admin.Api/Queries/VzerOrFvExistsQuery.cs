namespace AssociationRegistry.Admin.Api.Queries;

using Events;
using Framework;
using Marten;

public interface IVzerOrFvExistsQuery : IQuery<bool, VzerOrFvExistsFilter> { }

public class VzerOrFvExistsQuery : IVzerOrFvExistsQuery
{
    private readonly IDocumentSession _session;

    public VzerOrFvExistsQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<bool> ExecuteAsync(VzerOrFvExistsFilter existsFilter, CancellationToken cancellationToken)
    {
        var vzer = await _session
            .Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>()
            .SingleOrDefaultAsync(x => x.VCode == existsFilter.VCode, token: cancellationToken);

        var fv = await _session
            .Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens>()
            .SingleOrDefaultAsync(x => x.VCode == existsFilter.VCode, token: cancellationToken);

        return vzer?.VCode is not null || fv?.VCode is not null;
    }
}

public record VzerOrFvExistsFilter
{
    public string VCode { get; }

    public VzerOrFvExistsFilter(string vCode)
    {
        VCode = vCode;
    }
}
