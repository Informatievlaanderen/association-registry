namespace AssociationRegistry.Test.E2E.Erkenningen.When_Schors_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using When_Erkenning_Werd_Geschorst;
using Xunit;

public class SchorsErkenningContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario, SchorsErkenningRequest>
{

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario InitializeScenario()
        => new();

    public SchorsErkenningContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario scenario)
    {
        CommandResult = await new SchorsErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(SchorsErkenningCollection))]
public class SchorsErkenningCollection : ICollectionFixture<SchorsErkenningContext>
{
}
