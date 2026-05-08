namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Schorsings_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class CorrigeerSchorsingErkenningContext
    : TestContextBase<ErkenningWerdGeschorstScenario, CorrigeerSchorsingErkenningRequest>
{
    protected override ErkenningWerdGeschorstScenario InitializeScenario() => new();

    public CorrigeerSchorsingErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeschorstScenario scenario)
    {
        CommandResult = await new CorrigeerSchorsinErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(CorrigeerSchorsingErkenningCollection))]
public class CorrigeerSchorsingErkenningCollection : ICollectionFixture<CorrigeerSchorsingErkenningContext> { }
