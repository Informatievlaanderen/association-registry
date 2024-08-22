namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_Sort_By_A_List
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_A_List(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("locaties")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "locaties"));
    }
}
