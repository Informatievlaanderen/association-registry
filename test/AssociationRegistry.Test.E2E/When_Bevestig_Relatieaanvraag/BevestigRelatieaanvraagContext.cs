namespace AssociationRegistry.Test.E2E.When_Bevestig_Relatieaanvraag;

using Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.Relatieaanvragen;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[CollectionDefinition(nameof(BevestigRelatieaanvraagCollection))]
public class BevestigRelatieaanvraagCollection : ICollectionFixture<BevestigRelatieaanvraagContext> { }

/// <summary>The koepel asks the vereniging to become lid, and the vereniging confirms.</summary>
public class BevestigRelatieaanvraagContext
    : TestContextBase<KoepelEnVerenigingScenario, BevestigRelatieaanvraagRequest>
{
    public BevestigRelatieaanvraagContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    public string AanvraagId { get; private set; } = null!;

    protected override KoepelEnVerenigingScenario InitializeScenario() => new();

    protected override async ValueTask ExecuteScenario(KoepelEnVerenigingScenario scenario)
    {
        var indienen = new DienRelatieaanvraagInRequestFactory(scenario);
        await indienen.ExecuteRequest(ApiSetup);
        AanvraagId = indienen.AanvraagId;

        CommandResult = await new BevestigRelatieaanvraagRequestFactory(
            AanvraagId,
            scenario.VerenigingVCode
        ).ExecuteRequest(ApiSetup);
    }
}
