namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using Fixtures;
using FluentAssertions;
using Xunit;
using AssociationRegistry.Public.Api.Constants;
using Events;

public class Given_A_Vereniging_With_Actual_Minimal_Data_Fixture : PublicApiFixture
{
    public const string VCode = "v000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public Given_A_Vereniging_With_Actual_Minimal_Data_Fixture() : base(nameof(Given_A_Vereniging_With_Actual_Minimal_Data_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AddEvent(VCode, new VerenigingWerdGeregistreerd(VCode, Naam, null, null, null, null, "Actief", DateTime.Today));
    }
}

public class Given_A_Vereniging_With_Actual_Minimal_Data : IClassFixture<Given_A_Vereniging_With_Actual_Minimal_Data_Fixture>
{
    private const string VCode = "v000001";
    private readonly HttpClient _httpClient;

    public Given_A_Vereniging_With_Actual_Minimal_Data(Given_A_Vereniging_With_Actual_Minimal_Data_Fixture fixture)
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
            $"{nameof(Given_A_Vereniging_With_Actual_Minimal_Data)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
