namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Sort_By_NestedField
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_Sort_By_NestedField(EventsInDbScenariosFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    [InlineData("doelgroep.minimumleeftijd")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _adminApiClient.Search(q: "*", field);
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInAscendingOrder();
        names.ToList().ForEach(_outputHelper.WriteLine);
    }

    [Theory]
    [InlineData("verenigingstype.code")]
    [InlineData("doelgroep.minimumleeftijd")]
    public async Task? Then_it_sorts_descending(string field)
    {
        var response = await _adminApiClient.Search(q: "*", $"-{field}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var names = jToken.SelectTokens($"$.verenigingen[*].{field}")
                          .Select(x => x.Value<string>())
                          .ToList();

        names.Should().NotBeEmpty();
        names.Should().BeInDescendingOrder();
        names.ForEach(_outputHelper.WriteLine);
    }
}
