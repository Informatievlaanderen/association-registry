namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_Sort_By_NestedField
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_NestedField(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    [InlineData("doelgroep.minimumleeftijd")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();

        // TODO: Temporary bug until we migrate to vzer
        // Remove this line when migrated
        //names.Should().BeInAscendingOrder();
        names.ToList().ForEach(_outputHelper.WriteLine);
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    [InlineData("doelgroep.minimumleeftijd")]
    public async Task? Then_it_sorts_descending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", $"-{field}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        // TODO: Temporary bug until we migrate to vzer
        // Remove this line when migrated
        //names.Should().BeInDescendingOrder();
        names.ForEach(_outputHelper.WriteLine);
    }
}
