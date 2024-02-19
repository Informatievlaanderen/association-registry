namespace AssociationRegistry.Test.Admin.Api.When_Searching.With_Sorting;

using Fixtures;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_No_Sort
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_No_Sort(EventsInDbScenariosFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task? Then_it_sorts_by_vcode_descending()
    {
        var response = await _adminApiClient.Search("*");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);
        var vCodes = jToken.SelectTokens("$.verenigingen[*].vCode").Select(x => x.Value<string>());

        vCodes.Should().NotBeEmpty();
        vCodes.Should().BeInDescendingOrder();
        vCodes.ToList().ForEach(_outputHelper.WriteLine);
    }
}
