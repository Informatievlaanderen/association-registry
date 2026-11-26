namespace AssociationRegistry.Scheduled.Host.Queries;

using Admin.Schema.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen;
using Framework;
using Marten;

public interface ITeVervallenRelatieaanvragenQuery : IQuery<IReadOnlyList<RelatieaanvraagDocument>>;

/// <summary>
///     Reads the projection: the guards do not know the vervaldatum. The status is filtered in the database, the
///     vervaldatum in memory with the rule of the domain. When the projection is behind, a relatieaanvraag is found a day
///     later: the handler decides on the stream.
/// </summary>
public class TeVervallenRelatieaanvragenQuery : ITeVervallenRelatieaanvragenQuery
{
    private readonly IDocumentSession _session;
    private readonly IClock _clock;

    public TeVervallenRelatieaanvragenQuery(IDocumentSession session, IClock clock)
    {
        _session = session;
        _clock = clock;
    }

    public async Task<IReadOnlyList<RelatieaanvraagDocument>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var aangevraagd = RelatieaanvraagStatus.Aangevraagd.Value;
        var today = _clock.Today;

        var openRelatieaanvragen = await _session
            .Query<RelatieaanvraagDocument>()
            .Where(relatieaanvraag => relatieaanvraag.Status == aangevraagd)
            .ToListAsync(token: cancellationToken);

        return openRelatieaanvragen
            .Where(relatieaanvraag => Relatieaanvraag.IsTermijnVerstreken(relatieaanvraag.VervalDatum, today))
            .ToArray();
    }
}
