namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Vereniging;
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

    [Category(Categories.RefactorAfterVZERMigration)] // see skip
    [Theory(Skip = "Re-enable after migration")]
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

    [Category(Categories.RefactorAfterVZERMigration)] // see skip
    [Theory(Skip = "Re-enable after migration")]
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

        // TODO: Temporary bug until we migrate to vzer
        // Remove this line when migrated
        names = names.Select(x => x.Replace(Verenigingstype.FeitelijkeVereniging.Code, Verenigingstype.VZER.Code)).ToList();
        names.Should().NotBeEmpty();
        names.Should().BeInDescendingOrder();
        names.ForEach(_outputHelper.WriteLine);
    }
}
