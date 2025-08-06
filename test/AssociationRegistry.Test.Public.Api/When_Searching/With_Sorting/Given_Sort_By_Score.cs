namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using DecentraalBeheer.Vereniging;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using Resources;
using System.Net;
using Vereniging;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_Sort_By_Score
{
    private readonly GivenEventsFixture _fixture;
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;
    private string _query;
    private VCode _vCodeOfFullyMatched;

    public Given_Sort_By_Score(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;

        _query = SetUpQueryWithFullORPartialMatch();
        _vCodeOfFullyMatched = _fixture.V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario.VCode;
    }

    private string SetUpQueryWithFullORPartialMatch()
    {
        var exactMatch = _fixture.V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam;
        var partialMatch = _fixture.V003BasisgegevensWerdenGewijzigdScenario.NaamWerdGewijzigd.Naam.Split(" ")[0];

        var query = $"{exactMatch} OR {partialMatch}";

        return query;
    }

    [Theory]
    [InlineData("-score")]
    public async Task? Descending_Returns_FullMatch_First(string sortField)
    {
        var response = await _publicApiClient.Search(q: _query, sortField);

        var verenigingen = await AssertVerenigingenWereReturned(response);

        verenigingen.Verenigingen
                    .First()
                    .VCode
                    .Should()
                    .Be(_vCodeOfFullyMatched);
    }

    [Theory]
    [InlineData("score")]
    public async Task? Descending_Returns_FullMatch_Last(string sortField)
    {
        var searchResponse = await _publicApiClient.Search(q: _query, sortField);

        var verenigingen = await AssertVerenigingenWereReturned(searchResponse);

        verenigingen.Verenigingen
                    .Last()
                    .VCode
                    .Should().Be(_vCodeOfFullyMatched);
    }

    private async Task<SearchVerenigingenResponse?> AssertVerenigingenWereReturned(HttpResponseMessage response)
    {
        response.Should().BeSuccessful();

        var json = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(json);

        var verenigingen = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(json);

        verenigingen.Verenigingen.Should().HaveCountGreaterThan(1);

        return verenigingen;
    }
}
