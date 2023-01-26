namespace AssociationRegistry.Test.Public.Api.TakeTwo.Given_BasisGegevenWerdGewijzigd;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Framework;
using FluentAssertions;
using VCodes;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class When_Retrieving_Detail
{
    private readonly PublicApiClient _publicApiClient;
    private readonly VCode _vCode;

    public When_Retrieving_Detail(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.KorteBeschrijvingWerdGewijzigdScenario.VCode;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response_with_the_new_korteBeschrijving()
    {
        var responseMessage = await _publicApiClient.GetDetail(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(When_Retrieving_Detail)}_{nameof(Then_we_get_a_detail_vereniging_response_with_the_new_korteBeschrijving)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
