<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/With_Sorting/Given_A_Field_With_Inconclusive_Order.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching.With_Sorting;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching.With_Sorting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/With_Sorting/Given_A_Field_With_Inconclusive_Order.cs

using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Field_With_Inconclusive_Order
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Field_With_Inconclusive_Order(EventsInDbScenariosFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    public async Task? Then_it_sorts_by_field_then_by_vCode_descending(string field)
    {
        var response1 = await _adminApiClient.Search(q: "*");
        var content1 = await response1.Content.ReadAsStringAsync();

        var response = await _adminApiClient.Search(q: "*", field);
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var groups = jToken.SelectTokens("$.verenigingen[*]")
                           .Select(x => (x.SelectToken($".{field}").Value<string>(),
                                         x.SelectToken(".vCode").Value<string>()))
                           .GroupBy(keySelector: x => x.Item1, elementSelector: x => x.Item2)
                           .ToDictionary(keySelector: x => x.Key, elementSelector: x => x.ToList());

        groups.Keys.Should().NotBeEmpty();
        groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInDescendingOrder();
            group.Value.ForEach(_outputHelper.WriteLine);
        }
    }
}
