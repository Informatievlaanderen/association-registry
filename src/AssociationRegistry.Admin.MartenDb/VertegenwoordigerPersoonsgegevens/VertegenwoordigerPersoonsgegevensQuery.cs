namespace AssociationRegistry.Admin.MartenDb.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using Marten;

public class VertegenwoordigerPersoonsgegevensQuery : IVertegenwoordigerPersoonsgegevensQuery
{
    private readonly IQuerySession _session;

    public VertegenwoordigerPersoonsgegevensQuery(IQuerySession session)
    {
        _session = session;
    }

    public async Task<VertegenwoordigerPersoonsgegevensDocument?> ExecuteAsync(VertegenwoordigerPersoonsgegevensFilter filter, CancellationToken cancellationToken)
        => await _session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                         .ForRefId(filter.RefId)
                         .SingleOrDefaultAsync(token: cancellationToken);
}
