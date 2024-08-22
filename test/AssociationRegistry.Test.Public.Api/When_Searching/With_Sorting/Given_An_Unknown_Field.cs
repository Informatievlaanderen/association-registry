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
public class Given_Sort_By_UnknownField
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_UnknownField(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task? Then_it_returns_an_error_message_for_the_first_wrong_field()
    {
        var unknownField = "vCode,asdfasdfasdfasdf,balk";
        var response = await _publicApiClient.Search(q: "*", unknownField);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "asdfasdfasdfasdf"));
    }
}
