namespace AssociationRegistry.Test.E2E.When_Trek_Relatieaanvraag_In;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.Relatieaanvragen;
using Scenarios.Requests;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[CollectionDefinition(nameof(TrekRelatieaanvraagInCollection))]
public class TrekRelatieaanvraagInCollection : ICollectionFixture<TrekRelatieaanvraagInContext> { }

/// <summary>The koepel asks the vereniging to become lid, and withdraws its relatieaanvraag.</summary>
public class TrekRelatieaanvraagInContext : TestContextBase<KoepelEnVerenigingScenario, NullRequest>
{
    public TrekRelatieaanvraagInContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    public string AanvraagId { get; private set; } = null!;

    protected override KoepelEnVerenigingScenario InitializeScenario() => new();

    protected override async ValueTask ExecuteScenario(KoepelEnVerenigingScenario scenario)
    {
        var indienen = new DienRelatieaanvraagInRequestFactory(scenario);
        await indienen.ExecuteRequest(ApiSetup);
        AanvraagId = indienen.AanvraagId;

        CommandResult = await new TrekRelatieaanvraagInRequestFactory(AanvraagId, scenario.KoepelVCode).ExecuteRequest(
            ApiSetup
        );
    }
}
