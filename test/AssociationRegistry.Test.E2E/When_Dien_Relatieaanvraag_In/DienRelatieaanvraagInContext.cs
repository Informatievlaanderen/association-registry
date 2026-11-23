namespace AssociationRegistry.Test.E2E.When_Dien_Relatieaanvraag_In;

using Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.Relatieaanvragen;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[CollectionDefinition(nameof(DienRelatieaanvraagInCollection))]
public class DienRelatieaanvraagInCollection : ICollectionFixture<DienRelatieaanvraagInContext> { }

public class DienRelatieaanvraagInContext : TestContextBase<KoepelEnVerenigingScenario, DienRelatieaanvraagInRequest>
{
    public DienRelatieaanvraagInContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    public string AanvraagId { get; private set; } = null!;

    protected override KoepelEnVerenigingScenario InitializeScenario() => new();

    protected override async ValueTask ExecuteScenario(KoepelEnVerenigingScenario scenario)
    {
        var requestFactory = new DienRelatieaanvraagInRequestFactory(scenario);

        CommandResult = await requestFactory.ExecuteRequest(ApiSetup);
        AanvraagId = requestFactory.AanvraagId;
    }
}
