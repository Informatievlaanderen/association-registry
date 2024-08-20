<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/With_Sorting/Given_Sort_By_A_List.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching.With_Sorting;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching.With_Sorting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/With_Sorting/Given_Sort_By_A_List.cs

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using Resources;
using System.Net;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Sort_By_A_List
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_Sort_By_A_List(EventsInDbScenariosFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Theory]
    [InlineData("locaties")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _adminApiClient.Search(q: "*", field);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "locaties"));
    }
}
