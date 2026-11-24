namespace AssociationRegistry.Test.E2E.When_Weiger_Relatieaanvraag;

using Scenarios.Requests.Relatieaanvragen;
using Xunit;

/// <summary>
///     The refusal released the pair: the koepel can ask again. The factory expects 202. The other tests of the collection
///     use the refused relatieaanvraag, so this new one does not change them.
/// </summary>
[Collection(nameof(WeigerRelatieaanvraagCollection))]
public class Accepts_A_New_Relatieaanvraag_For_The_Same_Pair
{
    private readonly WeigerRelatieaanvraagContext _testContext;

    public Accepts_A_New_Relatieaanvraag_For_The_Same_Pair(WeigerRelatieaanvraagContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask Then_202() =>
        await new DienRelatieaanvraagInRequestFactory(_testContext.Scenario).ExecuteRequest(_testContext.ApiSetup);
}
