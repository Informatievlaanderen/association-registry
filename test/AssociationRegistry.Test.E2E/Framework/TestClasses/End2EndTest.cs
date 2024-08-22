namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;

public abstract class End2EndTest<TContext, TRequest, TResponse> where TContext : IEnd2EndContext<TRequest>
{
    private readonly TContext _context;

    protected End2EndTest(TContext context)
    {
        _context = context;

        Response = GetResponse(context.QueryApiHost)!;
    }

    protected string VCode => _context.VCode;
    protected TRequest Request => _context.Request;
    protected TResponse Response { get; init; }
    protected abstract Func<IAlbaHost, TResponse> GetResponse { get; }
}
