namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using Resources;
using System.Net;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_Sort_By_Score
{
    private readonly GivenEventsFixture _fixture;
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_Score(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("score")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var response = await _publicApiClient.Search(q: _fixture.V003BasisgegevensWerdenGewijzigdScenario.NaamWerdGewijzigd.Naam, field);

        response.Should().BeSuccessful();

        var json = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(json);

        var verenigingen = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(json);

        // just a checkup
        verenigingen.Verenigingen.Should().HaveCountGreaterThan(1);

        verenigingen.Verenigingen.First().VCode.Should().Be(_fixture.V003BasisgegevensWerdenGewijzigdScenario.VCode);
    }
}
