namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using Fixtures;
using FluentAssertions;
using Xunit;
using AssociationRegistry.Public.Api.Constants;
using Events;

public class Given_a_vereniging_with_actual_minimal_data_fixture : PublicApiFixture
{
    public const string VCode = "v000001";
    public const string Naam = "Feestcommittee Oudenaarde";

    public Given_a_vereniging_with_actual_minimal_data_fixture() : base(nameof(Given_a_vereniging_with_actual_minimal_data_fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AddEvent(VCode, new VerenigingWerdGeregistreerd(VCode, Naam));
    }
}

public class Given_a_vereniging_with_actual_minimal_data : IClassFixture<Given_a_vereniging_with_actual_minimal_data_fixture>
{
    private readonly HttpClient _httpClient;
    public const string VCode = Given_a_vereniging_with_actual_minimal_data_fixture.VCode;

    public Given_a_vereniging_with_actual_minimal_data(Given_a_vereniging_with_actual_minimal_data_fixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}");
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}");

        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_a_vereniging_with_actual_minimal_data)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
