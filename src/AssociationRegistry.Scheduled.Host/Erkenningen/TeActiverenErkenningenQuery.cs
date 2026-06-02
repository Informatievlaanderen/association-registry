namespace AssociationRegistry.Scheduled.Host.Erkenningen;

using System.Globalization;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Marten;
using DomainErkenningsPeriode = AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.ErkenningsPeriode;
using DomainErkenningStatus = AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.ErkenningStatus;
using IClock = NodaTime.IClock;

public record TeActiverenErkenning(VCode VCode, int ErkenningId);

public interface ITeActiverenErkenningenQuery : IQuery<IReadOnlyList<TeActiverenErkenning>>;

public class TeActiverenErkenningenQuery : ITeActiverenErkenningenQuery
{
    private readonly IDocumentSession _session;
    private readonly IClock _clock;

    public TeActiverenErkenningenQuery(IDocumentSession session, IClock clock)
    {
        _session = session;
        _clock = clock;
    }

    public async Task<IReadOnlyList<TeActiverenErkenning>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc());

        var verenigingen = await _session
            .Query<BeheerVerenigingDetailDocument>()
            .Where(vereniging => !vereniging.Deleted)
            .ToListAsync(token: cancellationToken);

        return verenigingen
            .Where(vereniging => vereniging.Status is not "Dubbel")
            .SelectMany(vereniging =>
                vereniging
                    .Erkenningen.Where(erkenning => erkenning.KanGeactiveerdWordenOp(today))
                    .Select(erkenning => new TeActiverenErkenning(
                        VCode.Hydrate(vereniging.VCode),
                        erkenning.ErkenningId
                    ))
            )
            .ToArray();
    }
}

file static class ErkenningExtensions
{
    public static bool KanGeactiveerdWordenOp(this Erkenning erkenning, DateOnly today)
    {
        if (erkenning.Status != DomainErkenningStatus.InAanvraag.Value)
            return false;

        if (erkenning.Startdatum is null)
            return false;

        var periode = DomainErkenningsPeriode.Hydrate(ParseDate(erkenning.Startdatum), ParseDate(erkenning.Einddatum));

        return DomainErkenningStatus.Bepaal(periode, today) == DomainErkenningStatus.Actief;
    }

    private static DateOnly? ParseDate(string? date) =>
        date is null ? null : DateOnly.Parse(date, CultureInfo.InvariantCulture);
}
