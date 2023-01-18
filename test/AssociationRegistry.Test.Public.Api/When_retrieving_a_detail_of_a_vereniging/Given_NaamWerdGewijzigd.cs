namespace AssociationRegistry.Test.Public.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Text.RegularExpressions;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Public.Api.Constants;
using Xunit;

public class Given_NaamWerdGewijzigd_Fixture : PublicApiFixture
{
    public const string VCode = "V000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public Given_NaamWerdGewijzigd_Fixture() : base(nameof(Given_NaamWerdGewijzigd_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                "Foudenaarder feest",
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)));
        await AddEvent(
            VCode,
            new NaamWerdGewijzigd(VCode, Naam));
    }
}

public class Given_NaamWerdGewijzigd : IClassFixture<Given_NaamWerdGewijzigd_Fixture>
{
    private const string VCode = "V000001";
    private readonly PublicApiClient _publicApiClient;

    public Given_NaamWerdGewijzigd(Given_NaamWerdGewijzigd_Fixture werdGewijzigdFixture)
    {
        _publicApiClient = werdGewijzigdFixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(VCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _publicApiClient.GetDetail(VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_VerenigingWerdGeregistreerd)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
