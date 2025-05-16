namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Xunit;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_VerenigingsType
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_VerenigingsType( SearchContext testContext)

    {
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask Then_it_sorts_by_Verenigingstype_then_by_vCode_descending()
    {
        var response = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                                    "*&sort=verenigingstype.code",
                                                                                    headers: new RequestParameters().V2());

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
