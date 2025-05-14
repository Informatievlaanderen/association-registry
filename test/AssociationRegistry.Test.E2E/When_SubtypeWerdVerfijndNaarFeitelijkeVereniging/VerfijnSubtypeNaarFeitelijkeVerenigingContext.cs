namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Scenarios.Requests.VZER;
using Vereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class VerfijnSubtypeNaarFeitelijkeVerenigingCollection : ICollectionFixture<VerfijnSubtypeNaarFeitelijkeVerenigingContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class VerfijnSubtypeNaarFeitelijkeVerenigingContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, WijzigSubtypeRequest>
{
    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VerfijnSubtypeNaarFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigSubtypeRequestVoorVerfijnNaarFvFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
