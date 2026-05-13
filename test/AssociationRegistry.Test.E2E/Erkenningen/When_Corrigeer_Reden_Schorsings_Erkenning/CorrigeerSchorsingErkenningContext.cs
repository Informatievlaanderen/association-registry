namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Reden_Schorsings_Erkenning;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class CorrigeerSchorsingErkenningContext
    : TestContextBase<ErkenningWerdGeschorstScenario, CorrigeerSchorsingErkenningRequest>
{
    protected override ErkenningWerdGeschorstScenario InitializeScenario() => new();

    public CorrigeerSchorsingErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeschorstScenario scenario)
    {
        CommandResult = await new CorrigeerSchorsingErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(CorrigeerSchorsingErkenningCollection))]
public class CorrigeerSchorsingErkenningCollection : ICollectionFixture<CorrigeerSchorsingErkenningContext> { }
