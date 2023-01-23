namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging.Given_VerenigingWerdGeregistreerd;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.EventStore;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class With_Minimal_Fields_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;

    public With_Minimal_Fields_Fixture() : base(nameof(With_Minimal_Fields_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        VCode = _fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = null,
            KboNummer = null,
            Startdatum = null,
            KorteBeschrijving = null,
            ContactInfoLijst = Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),

        };
    }

    public StreamActionResult SaveVersionResult { get; private set; } = null!;
    public HttpResponseMessage Response { get; set; } = null!;

    protected override async Task Given()
    {
        var metadata = _fixture.Create<CommandMetadata>();
        SaveVersionResult = await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            metadata);
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetDetail(VCode);
    }
}

public class With_Minimal_Fields : IClassFixture<With_Minimal_Fields_Fixture>
{
    private readonly string _vCode;
    private readonly With_Minimal_Fields_Fixture _admingApiFixture;
    private readonly AdminApiClient _adminApiClient;

    public With_Minimal_Fields(With_Minimal_Fields_Fixture admingApiFixture)
    {
        _admingApiFixture = admingApiFixture;
        _vCode = admingApiFixture.VCode;
        _adminApiClient = admingApiFixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _admingApiFixture.SaveVersionResult.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _admingApiFixture.SaveVersionResult.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _admingApiFixture.Response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
{{
    ""vereniging"": {{
            ""vCode"": ""{_admingApiFixture.VCode}"",
            ""naam"": ""{_admingApiFixture.VerenigingWerdGeregistreerd.Naam}"",
            ""korteNaam"": null,
            ""korteBeschrijving"": null,
            ""kboNummer"": null,
            ""startdatum"": null,
            ""status"": ""Actief"",
            ""contactInfoLijst"": [],
            ""locaties"":[]
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": """"
        }}
        }}
";

        content.Should().BeEquivalentJson(expected);
    }
}
