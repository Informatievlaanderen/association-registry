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
    public async ValueTask Then_it_returns_an_error_message_for_the_first_wrong_field()
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
