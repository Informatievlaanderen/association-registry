namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_VerenigingsType
{
    private readonly SearchContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Given_Sorting_By_VerenigingsType( SearchContext testContext, ITestOutputHelper testOutputHelper)

    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "")]
    public async ValueTask Then_it_sorts_by_Verenigingstype_then_by_vCode_descending()
    {
        var response = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                                    "*&sort=verenigingstype.code",
                                                                                    _testContext.ApiSetup.AdminApiHost.DocumentStore(), headers: new RequestParameters().V2().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);

        var groups = response.Verenigingen.Select(x => new { x.VCode, x.Verenigingstype.Code })
                                 .GroupBy(x => x.Code, x => x.VCode)
                                 .ToDictionary(x => x.Key, x => x.ToList());

        groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInDescendingOrder();
        }
    }
}
