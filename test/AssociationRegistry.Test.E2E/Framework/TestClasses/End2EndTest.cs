namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using V2.When_Stop_Vereniging;
using Xunit;

public abstract class End2EndTest<TContext, TRequest, TResponse>: IClassFixture<TContext>, IAsyncLifetime where TContext : class, ITestContext<TRequest>
{
    public TContext Context { get; }
    // Convenience props
    public TRequest Request => Context.Request;
    public End2EndTest(TContext context)
    {
        Context = context;
    }

    public async Task InitializeAsync()
    {
        Response = GetResponse(Context.ApiSetup);
    }

    public TResponse Response { get; private set; }

    public abstract Func<IApiSetup, TResponse> GetResponse { get; }
    public async Task DisposeAsync()
    {
    }
}
