namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using When_Wijzig_Lidmaatschap;
using Xunit;

public abstract class End2EndTest<TResponse>: IAsyncLifetime
{
    private readonly FullBlownApiSetup _setup;
    public TResponse Response { get; private set; }

    public End2EndTest(FullBlownApiSetup setup)
    {
        _setup = setup;
    }

    public async ValueTask InitializeAsync()
    {
        Response = GetResponse(_setup);
    }

    public abstract TResponse GetResponse(FullBlownApiSetup setup);



    public async ValueTask DisposeAsync()
    {
    }
}
