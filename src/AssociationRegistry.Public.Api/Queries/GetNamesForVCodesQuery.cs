namespace AssociationRegistry.Public.Api.Queries;

using Framework;
using Marten;
using Schema.Detail;

public interface IGetNamesForVCodesQuery : IQuery<Dictionary<string, string>, GetNamesForVCodesFilter>;
public class GetNamesForVCodesQuery : IGetNamesForVCodesQuery
{
    private readonly IDocumentSession _session;

    public GetNamesForVCodesQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Dictionary<string, string>> ExecuteAsync(GetNamesForVCodesFilter filter, CancellationToken cancellationToken)
    {
        var beheerVerenigingDetailDocuments = await _session
                                                   .Query<PubliekVerenigingDetailDocument>()
                                                   .Where(x => filter.VCodes.Contains(x.VCode))
                                                   .Select(x => new { x.VCode, x.Naam})
                                                   .ToListAsync(token: cancellationToken);

        return beheerVerenigingDetailDocuments.ToDictionary(x => x.VCode, x => x.Naam);
    }
}

public record GetNamesForVCodesFilter
{
    public string[] VCodes { get; }

    public GetNamesForVCodesFilter(params string[] vCodes)
    {
        VCodes = vCodes;
    }
}
