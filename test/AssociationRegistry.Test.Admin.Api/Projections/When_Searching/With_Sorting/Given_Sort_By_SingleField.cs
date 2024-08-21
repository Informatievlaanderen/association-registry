namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching.With_Sorting;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Sort_By_SingleField
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_Sort_By_SingleField(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _adminApiClient.Search(q: "*", field);

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
        var response = await _adminApiClient.Search(q: "*", $"-{field}");
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
