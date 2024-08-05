namespace AssociationRegistry.Test.Admin.Api.Features.FeitelijkeVereniging.Hooks;

using AutoFixture;
using Fixtures;
using Framework;

[Binding]
public class RegistreerHook
{
    protected RegistreerHook()
    {
    }

    [BeforeTestRun]
    public static void BeforeTestRun(DefaultTestRunContext testRunContext)
        => testRunContext.Add(typeof(Fixture).FullName, new Fixture().CustomizeAdminApi());

    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        featureContext.Add(typeof(AdminApiClient).FullName, new AdminApiClient(new HttpClient()));
    }

    [BeforeScenario]
    public static void BeforeScenario(ScenarioContext scenarioContext)
    {
    }
}
