namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
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
    public async Task Then_it_sorts_by_vcode_descending()
    {
        var response = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoeken("*", _testContext.CommandResult.Sequence);
        var verenigingen = response.Verenigingen.Select(x => x.VCode);
        verenigingen.Should().BeInDescendingOrder();
    }
}
