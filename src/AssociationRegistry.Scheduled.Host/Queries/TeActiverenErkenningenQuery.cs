namespace AssociationRegistry.Scheduled.Host.Queries;

using System.Globalization;
using Admin.Schema.Detail;
using Admin.Schema.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Framework;
using Marten;

public interface ITeActiverenErkenningenQuery : IQuery<IReadOnlyList<ErkenningDocument>>;

public class TeActiverenErkenningenQuery : ITeActiverenErkenningenQuery
{
    private readonly IDocumentSession _session;
    private readonly IClock _clock;

    public TeActiverenErkenningenQuery(IDocumentSession session, IClock clock)
    {
        _session = session;
        _clock = clock;
    }

    public async Task<IReadOnlyList<ErkenningDocument>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var erkenningenInAanvraag = await _session
            .Query<ErkenningDocument>()
            .Where(erkenning => erkenning.Status == ErkenningStatus.InAanvraag.Value)
            .ToListAsync(token: cancellationToken);

        var teActiverenErkenningen = erkenningenInAanvraag.Where(erkenning => erkenning.Startdatum <= _clock.Today);

        return teActiverenErkenningen.ToArray();
    }
}
