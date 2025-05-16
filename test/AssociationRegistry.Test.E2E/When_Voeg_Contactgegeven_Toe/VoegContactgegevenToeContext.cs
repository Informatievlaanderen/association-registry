namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe;

using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class VoegContactgegevenToeContext: TestContextBase<VoegContactgegevenToeRequest>
{
    public const string Name = "VoegContactgegevenToeContext";
    public VCode VCode => RequestResult.VCode;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario Scenario { get; }

    public VoegContactgegevenToeContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new VoegContactgegevenToeRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
