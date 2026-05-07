namespace AssociationRegistry.Test.E2E.Erkenningen.When_Registreer_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class RegistreerErkenningContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, RegistreerErkenningRequest>
{

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public RegistreerErkenningContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new RegistreerErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(RegistreerErkenningCollection))]
public class RegistreerErkenningCollection : ICollectionFixture<RegistreerErkenningContext>
{
}
