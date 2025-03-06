namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.With_Sorting;

using AssociationRegistry.Resources;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Field_With_Incorrect_Casing
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Field_With_Incorrect_Casing(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task? Then_it_sorts_descending_via_vcode()
    {
        var incorrectlyCasedField = "VCODE";
        var response = await _adminApiClient.Search(q: "*", incorrectlyCasedField);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "VCODE"));
    }
}
