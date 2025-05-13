namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class RegistreerVerenigingMetRechtsperoonlijkheidCollection : ICollectionFixture<RegistreerVerenigingMetRechtsperoonlijkheidContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class RegistreerVerenigingMetRechtsperoonlijkheidContext : TestContextBase<EmptyScenario, RegistreerVerenigingUitKboRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public RegistreerVerenigingMetRechtsperoonlijkheidContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
        CommandResult = await new RegistreerVerenigingUitKboRequestFactory().ExecuteRequest(ApiSetup);
    }
}
