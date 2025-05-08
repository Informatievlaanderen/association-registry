namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using Xunit;

[Collection(nameof(SearchCollection))]
public class Given_No_Sort
{
    private readonly SearchContext _testContext;

    public Given_No_Sort( SearchContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void Then_it_sorts_by_vcode_descending()
    {
        var response = _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken( _testContext.ApiSetup.AdminHttpClient ,"*", _testContext.ApiSetup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.MaxSequenceByScenario)).GetAwaiter().GetResult();
        var verenigingen = response.Verenigingen.Select(x => x.VCode);
        verenigingen.Should().BeInDescendingOrder();
    }
}
