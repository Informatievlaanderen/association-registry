namespace AssociationRegistry.Test.Public.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Text.RegularExpressions;
using Fixtures;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using global::AssociationRegistry.Public.Api.Constants;
using NodaTime.Extensions;
using Vereniging;
using Xunit;

public class Given_A_Vereniging_With_All_Fields_Fixture : PublicApiFixture
{
    public const string VCode = "v000001";
    private const string Naam = "Feestcommittee Oudenaarde";
    private const string? KorteBeschrijving = "Het feestcommittee van Oudenaarde";
    private const string? KorteNaam = "FOud";
    private const string? KboNummer = "0123456789";
    private const string Initiator = "Een initiator";

    private readonly VerenigingWerdGeregistreerd.ContactInfo ContactInfo = new(
        "Algemeen",
        "info@FOud.be",
        "1111.11.11.11",
        "www.oudenaarde.be/feest",
        "#FOudenaarde");


    private DateOnly? Startdatum { get; } = DateOnly.FromDateTime(new DateTime(2022, 11, 9));


    public Given_A_Vereniging_With_All_Fields_Fixture() : base(nameof(Given_A_Vereniging_With_All_Fields_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam,
                KorteBeschrijving,
                Startdatum,
                KboNummer,
                new[] { ContactInfo },
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                Initiator,
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }
}

public class Given_A_Vereniging_With_All_Fields : IClassFixture<Given_A_Vereniging_With_All_Fields_Fixture>
{
    private const string VCode = Given_A_Vereniging_With_All_Fields_Fixture.VCode;
    private readonly HttpClient _httpClient;

    public Given_A_Vereniging_With_All_Fields(Given_A_Vereniging_With_All_Fields_Fixture fixture)
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
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Vereniging_With_All_Fields)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
