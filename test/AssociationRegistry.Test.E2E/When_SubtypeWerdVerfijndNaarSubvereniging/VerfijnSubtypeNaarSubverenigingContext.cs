namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Requests.VZER;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class VerfijnSubtypeNaarSubverenigingCollection : ICollectionFixture<VerfijnSubtypeNaarSubverenigingContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class VerfijnSubtypeNaarSubverenigingContext : TestContextBase<VzerAndKboVerenigingWerdenGeregistreerdScenario, WijzigSubtypeRequest>
{
    protected override VzerAndKboVerenigingWerdenGeregistreerdScenario InitializeScenario()
        => new();

    public VerfijnSubtypeNaarSubverenigingContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VzerAndKboVerenigingWerdenGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigSubtypeRequestVoorVerfijnNaarSubFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
