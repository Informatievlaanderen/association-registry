namespace AssociationRegistry.Test.E2E.When_Weiger_Relatieaanvraag;

using Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.Relatieaanvragen;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[CollectionDefinition(nameof(WeigerRelatieaanvraagCollection))]
public class WeigerRelatieaanvraagCollection : ICollectionFixture<WeigerRelatieaanvraagContext> { }

/// <summary>The koepel asks the vereniging to become lid, and the vereniging refuses.</summary>
public class WeigerRelatieaanvraagContext : TestContextBase<KoepelEnVerenigingScenario, WeigerRelatieaanvraagRequest>
{
    public WeigerRelatieaanvraagContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    public string AanvraagId { get; private set; } = null!;

    protected override KoepelEnVerenigingScenario InitializeScenario() => new();

    protected override async ValueTask ExecuteScenario(KoepelEnVerenigingScenario scenario)
    {
        var indienen = new DienRelatieaanvraagInRequestFactory(scenario);
        await indienen.ExecuteRequest(ApiSetup);
        AanvraagId = indienen.AanvraagId;

        CommandResult = await new WeigerRelatieaanvraagRequestFactory(
            AanvraagId,
            scenario.VerenigingVCode
        ).ExecuteRequest(ApiSetup);
    }
}
