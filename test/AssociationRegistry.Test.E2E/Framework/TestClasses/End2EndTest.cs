namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Xunit;

public abstract class End2EndTest<TContext, TRequest, TResponse>: IAsyncLifetime
    where TContext : TestContextBase<TRequest>
{
    public TContext TestContext { get; }
    // Convenience props
    public TRequest Request => TestContext.Request;
    public End2EndTest(TContext testContext)
    {
        TestContext = testContext;
    }

    public async ValueTask InitializeAsync()
    {
        Response = GetResponse(TestContext.ApiSetup);
    }

    public TResponse Response { get; private set; }

    public abstract Func<IApiSetup, TResponse> GetResponse { get; }
    public async ValueTask DisposeAsync()
    {
    }
}
