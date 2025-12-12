namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Schema.Vertegenwoordiger;

public interface IVertegenwoordigersPerVCodeQuery : IQuery<IReadOnlyList<VertegenwoordigersPerVCodeDocument>, VertegenwoordigersPerVCodeQueryFilter>;

public class VertegenwoordigersPerVCodeQuery : IVertegenwoordigersPerVCodeQuery
{
    private readonly IDocumentSession _session;

    public VertegenwoordigersPerVCodeQuery(IDocumentSession session)
        => _session = session;

    public async Task<IReadOnlyList<VertegenwoordigersPerVCodeDocument>> ExecuteAsync(
        VertegenwoordigersPerVCodeQueryFilter filter,
        CancellationToken cancellationToken)
    {
        IQueryable<VertegenwoordigersPerVCodeDocument> query =
            _session.Query<VertegenwoordigersPerVCodeDocument>();

        if (!string.IsNullOrWhiteSpace(filter.VCode))
        {
            query = query.Where(x => x.VCode == filter.VCode);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(
                x => x.VertegenwoordigersData.Any(d => d.Status == filter.Status)
            );
        }

        return await query.ToListAsync(token: cancellationToken);
    }
}

public record VertegenwoordigersPerVCodeQueryFilter
{
    public string? VCode { get; }
    public string? Status { get; }

    public VertegenwoordigersPerVCodeQueryFilter(string? vCode = null, string? status = null)
    {
        VCode = vCode;
        Status = status;
    }
}
