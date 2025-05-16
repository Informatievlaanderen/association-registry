﻿namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
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
    public void Then_it_sorts_by_Verenigingstype_then_by_vCode_descending()
    {
        var verenigingen = _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(
            _testContext.ApiSetup.SuperAdminHttpClient,"*&sort=verenigingstype.code", _testContext.MaxSequenceByScenario).GetAwaiter().GetResult().Verenigingen;

        var groups = verenigingen.Select(x => new { x.VCode, x.Verenigingstype.Code })
                                 .GroupBy(x => x.Code, x => x.VCode)
                                 .ToDictionary(x => x.Key, x => x.ToList());

        groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInDescendingOrder();
        }
    }
}
