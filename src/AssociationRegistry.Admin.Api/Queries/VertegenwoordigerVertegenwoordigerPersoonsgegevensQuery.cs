namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Schema.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensQuery : IQuery<VertegenwoordigerPersoonsgegevensDocument?, VertegenwoordigerPersoonsgegevensFilter>;

public class VertegenwoordigerPersoonsgegevensQuery : IVertegenwoordigerPersoonsgegevensQuery
{
    private readonly IDocumentSession _session;

    public VertegenwoordigerPersoonsgegevensQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<VertegenwoordigerPersoonsgegevensDocument?> ExecuteAsync(VertegenwoordigerPersoonsgegevensFilter filter, CancellationToken cancellationToken)
        => await _session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                         .ForRefId(filter.RefId)
                         .SingleOrDefaultAsync(token: cancellationToken);
}

public record VertegenwoordigerPersoonsgegevensFilter
{
    public Guid RefId { get; }

    public VertegenwoordigerPersoonsgegevensFilter(Guid refId)
    {
        RefId = refId;
    }
}
