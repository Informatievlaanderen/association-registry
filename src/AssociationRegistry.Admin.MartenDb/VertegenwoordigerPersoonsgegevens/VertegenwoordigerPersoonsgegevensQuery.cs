namespace AssociationRegistry.Admin.MartenDb.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using Marten;

public class VertegenwoordigerPersoonsgegevensQuery : IVertegenwoordigerPersoonsgegevensQuery
{
    private readonly IDocumentSession _session;

    public VertegenwoordigerPersoonsgegevensQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<VertegenwoordigerPersoonsgegevensDocument?> ExecuteAsync(
        VertegenwoordigerPersoonsgegevensFilter filter,
        CancellationToken cancellationToken)
        => _session.PendingChanges.Inserts().OfType<VertegenwoordigerPersoonsgegevensDocument>()
                   .SingleOrDefault(x => x.RefId == filter.RefId) ??
           await _session.LoadAsync<VertegenwoordigerPersoonsgegevensDocument>(filter.RefId, cancellationToken);
}
