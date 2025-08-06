namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(RegistreerFeitelijkeVerenigingWithPotentialDuplicatesCollection))]
public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext : TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, RegistreerFeitelijkeVerenigingRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario()
        => new();

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(scenario.FeitelijkeVerenigingWerdGeregistreerd).ExecuteRequest(ApiSetup);
    }
}
