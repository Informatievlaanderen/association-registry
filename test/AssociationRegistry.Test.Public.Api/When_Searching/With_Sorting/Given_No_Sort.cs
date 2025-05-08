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
public class Given_No_Sort
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_No_Sort(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask? Then_it_sorts_by_vcode_descending()
    {
        var response = await _publicApiClient.Search("*");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);
        var vCodes = jToken.SelectTokens("$.verenigingen[*].vCode").Select(x => x.Value<string>());

        vCodes.Should().NotBeEmpty();
        vCodes.Should().BeInDescendingOrder();
        vCodes.ToList().ForEach(_outputHelper.WriteLine);
    }
}
