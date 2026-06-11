namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Reden_Schorsings_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class CorrigeerRedenSchorsingErkenningContext
    : TestContextBase<ErkenningWerdGeschorstScenario, CorrigeerRedenSchorsingErkenningRequest>
{
    protected override ErkenningWerdGeschorstScenario InitializeScenario() => new();

    public CorrigeerRedenSchorsingErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeschorstScenario scenario)
    {
        CommandResult = await new CorrigeerSchorsingErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(CorrigeerRedenSchorsingErkenningCollection))]
public class CorrigeerRedenSchorsingErkenningCollection
    : ICollectionFixture<CorrigeerRedenSchorsingErkenningContext> { }
