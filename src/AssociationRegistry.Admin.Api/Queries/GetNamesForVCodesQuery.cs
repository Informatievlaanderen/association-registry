namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Schema.Detail;

public class GetNamesForVCodesQuery : IQuery<Dictionary<string, string>, GetNamesForVCodesFilter>
{
    private readonly IDocumentSession _session;

    public GetNamesForVCodesQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Dictionary<string, string>> ExecuteAsync(GetNamesForVCodesFilter filter, CancellationToken cancellationToken)
    {
        var beheerVerenigingDetailDocuments = await _session
                                                   .Query<BeheerVerenigingDetailDocument>()
                                                   .Where(x => filter.VCodes.Contains(x.VCode))
                                                   .Select(x => new { x.VCode, x.Naam})
                                                   .ToListAsync(token: cancellationToken);

        return beheerVerenigingDetailDocuments.ToDictionary(x => x.VCode, x => x.Naam);
    }
}

public class GetNamesForVCodesFilter
{
    public string[] VCodes { get; }

    public GetNamesForVCodesFilter(params string[] vCodes)
    {
        VCodes = vCodes;
    }
}
