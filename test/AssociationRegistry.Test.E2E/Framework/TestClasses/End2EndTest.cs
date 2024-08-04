namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;
using AlbaHost;

public abstract class End2EndTest<TContext, TRequest, TResponse> where TContext: IEnd2EndContext<TRequest>
{
    private readonly TContext _context;

    protected End2EndTest(TContext context)
    {
        _context = context;

        var adminApiHost = context.AdminApiHost;

        Response = GetResponse(adminApiHost)!;
    }

    protected string VCode => _context.ResultingVCode;
    protected TRequest Request => _context.Request;
    protected TResponse Response { get; init; }

    protected abstract Func<IAlbaHost, TResponse> GetResponse { get; }
}
