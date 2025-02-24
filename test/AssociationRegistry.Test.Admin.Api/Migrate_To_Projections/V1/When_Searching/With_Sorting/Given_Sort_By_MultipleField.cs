namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Sort_By_MultipleFields
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;
    private IElasticClient _elasticClient;

    public Given_Sort_By_MultipleFields(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _adminApiClient = fixture.AdminApiClient;
        _elasticClient = fixture.ElasticClient;
    }

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task? Then_it_sorts_ascending_then_descending(string ascendingField, string descendingField)
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var response = await _adminApiClient.Search(q: "*", $"{ascendingField},-{descendingField}");

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
            group.Value.Should().BeInDescendingOrder(new CaseInsensitiveComparer());
            group.Value.ForEach(_outputHelper.WriteLine);
        }
    }

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task? Then_it_sorts_descending_then_ascending(string descendingField, string ascendingField)
    {
        await _elasticClient.Indices.RefreshAsync();

        var response = await _adminApiClient.Search(q: "*", $"-{descendingField},{ascendingField}");
        var content = await response.Content.ReadAsStringAsync();

        var jToken = JToken.Parse(content);

        var groups = jToken.SelectTokens("$.verenigingen[*]")
                           .Select(x => (x.SelectToken($".{descendingField}").Value<string>(),
                                         x.SelectToken($".{ascendingField}").Value<string>()))
                           .GroupBy(keySelector: x => x.Item1, elementSelector: x => x.Item2)
                           .ToDictionary(keySelector: x => x.Key, elementSelector: x => x.ToList());

        // TODO: Temporary bug until we migrate to vzer
        // Remove this line when migrated
        var keyCollection = groups.Keys.Select(x => x.Replace(Verenigingstype.FeitelijkeVereniging.Code, Verenigingstype.VZER.Code));

        keyCollection.Should().NotBeEmpty();
        keyCollection.Should().BeInDescendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInAscendingOrder(new CaseInsensitiveComparer());
            group.Value.ForEach(_outputHelper.WriteLine);
        }
    }
}
