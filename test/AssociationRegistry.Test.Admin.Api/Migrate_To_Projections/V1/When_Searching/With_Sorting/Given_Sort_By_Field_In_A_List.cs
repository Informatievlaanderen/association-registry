namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(AdminApiCollection))]
public class Given_Sort_By_Field_In_A_List
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_Sort_By_Field_In_A_List(EventsInDbScenariosFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Theory]
    [InlineData("locaties.postcode")]
    public async ValueTask Then_it_returns200_but_we_dont_support_it(string field)
    {
        var response = await _adminApiClient.Search(q: "*", field);

        response.Should().BeSuccessful();
    }
}
