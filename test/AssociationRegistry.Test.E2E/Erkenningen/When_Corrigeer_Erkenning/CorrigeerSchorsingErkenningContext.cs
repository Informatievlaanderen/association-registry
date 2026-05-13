namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class CorrigeerErkenningContext
    : TestContextBase<ErkenningWerdGeregistreerdScenario, CorrigeerErkenningRequest>
{
    protected override ErkenningWerdGeregistreerdScenario InitializeScenario() => new();

    public CorrigeerErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new CorrigeerErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(CorrigeerErkenningCollection))]
public class CorrigeerErkenningCollection : ICollectionFixture<CorrigeerErkenningContext> { }
