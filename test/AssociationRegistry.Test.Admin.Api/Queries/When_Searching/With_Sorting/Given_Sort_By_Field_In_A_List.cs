<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/With_Sorting/Given_Sort_By_Field_In_A_List.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching.With_Sorting;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching.With_Sorting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/With_Sorting/Given_Sort_By_Field_In_A_List.cs

using FluentAssertions;
using Framework.Fixtures;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
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
    public async Task? Then_it_returns200_but_we_dont_support_it(string field)
    {
        var response = await _adminApiClient.Search(q: "*", field);

        response.Should().BeSuccessful();
    }
}
