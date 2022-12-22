namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using System.Text.RegularExpressions;
using AssociationRegistry.Framework;
using Fixtures;
using Vereniging;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

public class Given_A_Vereniging_Fixture : AdminApiFixture
{
    public const string VCode = "v000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public Given_A_Vereniging_Fixture() : base(nameof(Given_A_Vereniging_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
        new CommandMetadata(
            Initiator: "Een initiator",
            Tijdstip: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero).ToInstant()));
    }
}

public class Given_A_Vereniging : IClassFixture<Given_A_Vereniging_Fixture>
{
    private const string VCode = "v000001";
    private readonly HttpClient _httpClient;

    public Given_A_Vereniging(Given_A_Vereniging_Fixture fixture)
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
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}");

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Vereniging)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
