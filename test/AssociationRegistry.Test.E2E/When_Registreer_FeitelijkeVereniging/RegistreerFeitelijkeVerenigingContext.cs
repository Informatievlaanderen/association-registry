namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging;

using Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(RegistreerFeitelijkeVerenigingCollection))]
public class RegistreerFeitelijkeVerenigingCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class RegistreerFeitelijkeVerenigingContext : TestContextBase<EmptyScenario, RegistreerFeitelijkeVerenigingRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public RegistreerFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
        CommandResult = await new RegistreerFeitelijkeVerenigingRequestFactory().ExecuteRequest(ApiSetup);
    }
}
