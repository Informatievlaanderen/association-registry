namespace AssociationRegistry.Scheduled.Host.Queries;

using Admin.Schema.Erkenningen;
using DecentraalBeheer.Vereniging.Erkenningen;
using Framework;
using Marten;

public interface ITeVerlopenErkenningenQuery : IQuery<IReadOnlyList<ErkenningDocument>>;

public class TeVerlopenErkenningenQuery : ITeVerlopenErkenningenQuery
{
    private readonly IDocumentSession _session;
    private readonly IClock _clock;

    public TeVerlopenErkenningenQuery(IDocumentSession session, IClock clock)
    {
        _session = session;
        _clock = clock;
    }

    public async Task<IReadOnlyList<ErkenningDocument>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var erkenningenVerlopen = await _session
            .Query<ErkenningDocument>()
            .Where(erkenning => erkenning.Status == ErkenningStatus.Actief.Value)
            .ToListAsync(token: cancellationToken);

        var teVerlopenErkenningen = erkenningenVerlopen.Where(erkenning => erkenning.Einddatum < _clock.Today);

        return teVerlopenErkenningen.ToArray();
    }
}
