namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_Sort_Ignore_Case
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_Ignore_Case(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);

        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInAscendingOrder(new CaseInsensitiveComparer());
        names.ForEach(_outputHelper.WriteLine);
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_descending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", $"-{field}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInDescendingOrder(new CaseInsensitiveComparer());
        names.ForEach(_outputHelper.WriteLine);
    }
}

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_Sort_Ignore_Leading_Spaces
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_Ignore_Leading_Spaces(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);

        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInAscendingOrder(new CaseInsensitiveComparer());
        names.ForEach(_outputHelper.WriteLine);
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_descending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", $"-{field}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInDescendingOrder(new CaseInsensitiveComparer());
        names.ForEach(_outputHelper.WriteLine);
    }
}
