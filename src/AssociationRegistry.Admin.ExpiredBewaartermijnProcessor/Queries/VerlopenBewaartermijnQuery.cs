namespace AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Queries;

using Framework;
using Marten;
using NodaTime;
using Schema.Bewaartermijn;

public interface IVerlopenBewaartermijnQuery : IQuery<IReadOnlyList<BewaartermijnDocument>>;

public class VerlopenBewaartermijnQuery : IVerlopenBewaartermijnQuery
{
    private readonly IDocumentSession _session;

    public VerlopenBewaartermijnQuery(IDocumentSession session)
        => _session = session;

    public async Task<IReadOnlyList<BewaartermijnDocument>> ExecuteAsync(CancellationToken cancellationToken)
    {
        IQueryable<BewaartermijnDocument> query =
            _session.Query<BewaartermijnDocument>();

        var now = SystemClock.Instance.GetCurrentInstant();

        var geplandeBewaartermijnen = await query
                    .Where(x=>x.Status == BewaartermijnStatus.Gepland.StatusNaam)
                    .ToListAsync(token: cancellationToken);

        var verlopenBewaartermijnen = geplandeBewaartermijnen
           .Where(x => x.Vervaldag < now); // Marten cannot support custom value types in Linq expression like 'Instant'

        // TODO uitzoeken op welke manier dit voor de .ToListAsync() kan gebruikt worden

        return verlopenBewaartermijnen.ToArray();
    }
}


