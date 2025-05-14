namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
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
        var response = _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken( _testContext.ApiSetup.AdminHttpClient ,"*", headers: new RequestParameters()).GetAwaiter().GetResult();
        var verenigingen = response.Verenigingen.Select(x => x.VCode);
        verenigingen.Should().BeInDescendingOrder();
    }
}
