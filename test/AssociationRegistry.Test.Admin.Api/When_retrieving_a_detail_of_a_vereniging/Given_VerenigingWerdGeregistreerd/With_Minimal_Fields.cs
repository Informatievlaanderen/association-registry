namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging.Given_VerenigingWerdGeregistreerd;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.EventStore;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
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
    private readonly With_Minimal_Fields_Fixture _adminApiFixture;
    private readonly AdminApiClient _adminApiClient;

    public With_Minimal_Fields(With_Minimal_Fields_Fixture adminApiFixture)
    {
        _adminApiFixture = adminApiFixture;
        _vCode = adminApiFixture.VCode;
        _adminApiClient = adminApiFixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _adminApiFixture.SaveVersionResult.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _adminApiFixture.SaveVersionResult.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _adminApiFixture.Response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
{{
    ""vereniging"": {{
            ""vCode"": ""{_adminApiFixture.VCode}"",
            ""naam"": ""{_adminApiFixture.VerenigingWerdGeregistreerd.Naam}"",
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

    [Fact]
    public void Then_it_returns_an_etag_header()
    {
        _adminApiFixture.Response.Headers.ETag.Should().NotBeNull();
        var etagValues = _adminApiFixture.Response.Headers.GetValues(HeaderNames.ETag).ToList();
        etagValues.Should().HaveCount(1);
        var etag = etagValues[0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
