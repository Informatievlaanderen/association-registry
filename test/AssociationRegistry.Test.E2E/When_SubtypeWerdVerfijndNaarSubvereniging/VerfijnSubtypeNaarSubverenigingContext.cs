namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Requests.VZER;

public class VerfijnSubtypeNaarSubverenigingContext: TestContextBase<WijzigSubtypeRequest>
{
    public const string Name = "VerfijnSubtypeNaarSubverenigingContext";
    public VCode VCode => RequestResult.VCode;
    public VzerAndKboVerenigingWerdenGeregistreerdScenario Scenario { get; }

    public VerfijnSubtypeNaarSubverenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigSubtypeRequestVoorVerfijnNaarSubFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
