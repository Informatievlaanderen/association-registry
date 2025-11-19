namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(SearchCollection))]
public class Given_No_Sort
{
    private readonly SearchContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Given_No_Sort( SearchContext testContext, ITestOutputHelper testOutputHelper)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "")]
    public async Task Then_it_sorts_by_vcode_descending()
    {
        var response = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken( _testContext.ApiSetup.AdminHttpClient ,"*", _testContext.ApiSetup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);
        var verenigingen = response.Verenigingen.Select(x => x.VCode);
        verenigingen.Should().BeInDescendingOrder();
    }
}
