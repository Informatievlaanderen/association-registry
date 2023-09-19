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
public class Given_Sort_By_MultipleFields
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_MultipleFields(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("type.code", "naam")]
    [InlineData("type.code", "korteNaam")]
    [InlineData("type.code", "vCode")]
    public async Task? Then_it_sorts_ascending_then_descending(string ascendingField, string descendingField)
    {
        var response = await _publicApiClient.Search(q: "*", $"{ascendingField},-{descendingField}");

        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var groups = jToken.SelectTokens("$.verenigingen[*]")
                           .Select(x => (x.SelectToken($".{ascendingField}").Value<string>(),
                                         x.SelectToken($".{descendingField}").Value<string>()))
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

    [Theory]
    [InlineData("type.code", "naam")]
    [InlineData("type.code", "korteNaam")]
    [InlineData("type.code", "vCode")]
    public async Task? Then_it_sorts_descending_then_ascending(string ascendingField, string descendingField)
    {
        var response = await _publicApiClient.Search(q: "*", $"-{ascendingField},{descendingField}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var groups = jToken.SelectTokens("$.verenigingen[*]")
                           .Select(x => (x.SelectToken($".{ascendingField}").Value<string>(),
                                         x.SelectToken($".{descendingField}").Value<string>()))
                           .GroupBy(keySelector: x => x.Item1, elementSelector: x => x.Item2)
                           .ToDictionary(keySelector: x => x.Key, elementSelector: x => x.ToList());

        groups.Keys.Should().NotBeEmpty();
        groups.Keys.Should().BeInDescendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInAscendingOrder();
            group.Value.ForEach(_outputHelper.WriteLine);
        }
    }
}
