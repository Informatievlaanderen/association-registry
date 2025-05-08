namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging;

using Admin.Api.Verenigingen.Verwijder.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class VerwijderVerenigingContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, VerwijderVerenigingRequest>
{
    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VerwijderVerenigingContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
        => CommandResult = await new VerwijderVerenigingRequestFactory(scenario).ExecuteRequest(ApiSetup);
}

[CollectionDefinition(nameof(VerwijderVerenigingCollection))]
public class VerwijderVerenigingCollection : ICollectionFixture<VerwijderVerenigingContext>
{
}
