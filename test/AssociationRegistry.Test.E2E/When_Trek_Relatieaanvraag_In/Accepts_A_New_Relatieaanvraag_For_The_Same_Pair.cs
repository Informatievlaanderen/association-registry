namespace AssociationRegistry.Test.E2E.When_Trek_Relatieaanvraag_In;

using Scenarios.Requests.Relatieaanvragen;
using Xunit;

/// <summary>
///     The intrekking released the pair: the koepel can ask again. The factory expects 202. The other tests of the
///     collection use the withdrawn relatieaanvraag, so this new one does not change them.
/// </summary>
[Collection(nameof(TrekRelatieaanvraagInCollection))]
public class Accepts_A_New_Relatieaanvraag_For_The_Same_Pair
{
    private readonly TrekRelatieaanvraagInContext _testContext;

    public Accepts_A_New_Relatieaanvraag_For_The_Same_Pair(TrekRelatieaanvraagInContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask Then_202() =>
        await new DienRelatieaanvraagInRequestFactory(_testContext.Scenario).ExecuteRequest(_testContext.ApiSetup);
}
