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

public class VerfijnSubtypeNaarFeitelijkeVerenigingContext: TestContextBase<WijzigSubtypeRequest>
{
    public const string Name = "VerfijnSubtypeNaarFeitelijkeVerenigingContext";
    public VCode VCode => CommandResult.VCode;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario Scenario { get; }

    public VerfijnSubtypeNaarFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        CommandResult = await new WijzigSubtypeRequestVoorVerfijnNaarFvFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
