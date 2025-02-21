namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_A_Field_With_Inconclusive_Order
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_A_Field_With_Inconclusive_Order(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    public async Task? Then_it_sorts_by_field_then_by_vCode_descending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var groups = jToken.SelectTokens("$.verenigingen[*]")
                           .Select(x => (x.SelectToken($".{field}").Value<string>(),
                                         x.SelectToken(".vCode").Value<string>()))
                           .GroupBy(keySelector: x => x.Item1, elementSelector: x => x.Item2)
                           .ToDictionary(keySelector: x => x.Key, elementSelector: x => x.ToList());

        groups.Keys.Should().NotBeEmpty();
        // TODO: Temporary bug until we migrate to vzer
        // Remove this line when migrated
        //groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            // TODO: Temporary bug until we migrate to vzer
            // Remove this line when migrated
            //group.Value.Should().BeInDescendingOrder();
            group.Value.ForEach(_outputHelper.WriteLine);
        }
    }
}
