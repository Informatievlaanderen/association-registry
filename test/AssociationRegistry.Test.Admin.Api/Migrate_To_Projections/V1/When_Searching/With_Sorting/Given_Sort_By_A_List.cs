namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Resources;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(AdminApiCollection))]
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
