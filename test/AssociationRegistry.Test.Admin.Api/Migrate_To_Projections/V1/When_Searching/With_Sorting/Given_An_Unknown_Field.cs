namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching.With_Sorting;

using Resources;
using Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Sort_By_UnknownField
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AdminApiClient _adminApiClient;

    public Given_Sort_By_UnknownField(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task? Then_it_returns_an_error_message_for_the_first_wrong_field()
    {
        var unknownField = "vCode,asdfasdfasdfasdf,balk";
        var response = await _adminApiClient.Search(q: "*", unknownField);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "asdfasdfasdfasdf"));
    }
}
