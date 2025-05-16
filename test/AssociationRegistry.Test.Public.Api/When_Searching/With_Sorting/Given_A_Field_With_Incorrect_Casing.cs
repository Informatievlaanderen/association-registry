namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using Resources;
using System.Net;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_A_Field_With_Incorrect_Casing
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_A_Field_With_Incorrect_Casing(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _outputHelper = helper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task? Then_it_sorts_descending_via_vcode()
    {
        var incorrectlyCasedField = "VCODE";
        var response = await _publicApiClient.Search(q: "*", incorrectlyCasedField);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);

        responseContentObject.Detail.Should()
                             .Be(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, arg0: "VCODE"));
    }
}
