﻿namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Framework;
using VCodes;
using Xunit;

public class Given_A_Vereniging_With_A_Changed_Naam_Fixture : AdminApiFixture
{
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    private readonly CommandMetadata _metadata;

    public Given_A_Vereniging_With_A_Changed_Naam_Fixture() : base(nameof(Given_A_Vereniging_With_A_Changed_Naam_Fixture))
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        _metadata = fixture.Create<CommandMetadata>() with {ExpectedVersion = null};
    }

    public SaveChangesResult SaveResult { get; private set; } = null!;

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            _metadata);
        SaveResult = await AddEvent(
            VCode,
            NaamWerdGewijzigd,
            _metadata);
    }
}

public class Given_A_Vereniging_With_A_Changed_Naam : IClassFixture<Given_A_Vereniging_With_A_Changed_Naam_Fixture>
{
    private readonly string _vCode;
    private readonly Given_A_Vereniging_With_A_Changed_Naam_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Vereniging_With_A_Changed_Naam(Given_A_Vereniging_With_A_Changed_Naam_Fixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VCode;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.SaveResult.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.SaveResult.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _adminApiClient.GetDetail(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
        {{
            ""vereniging"": {{
                    ""vCode"": ""{_fixture.VCode}"",
                    ""naam"": ""{_fixture.NaamWerdGewijzigd.Naam}"",
                    ""korteNaam"": ""{_fixture.VerenigingWerdGeregistreerd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_fixture.VerenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""kboNummer"": ""{_fixture.VerenigingWerdGeregistreerd.KboNummer}"",
                    ""startdatum"": ""{_fixture.VerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactInfoLijst"": [{string.Join(',', _fixture.VerenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}""
                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _fixture.VerenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        { (x.Hoofdlocatie ? $"\"hoofdlocatie\": {x.Hoofdlocatie.ToString().ToLower()}," : string.Empty) }
                        ""adres"": ""{x.ToAdresString()}"",
                        ""naam"": ""{x.Naam}"",
                        ""straatnaam"": ""{x.Straatnaam}"",
                        ""huisnummer"": ""{x.Huisnummer}"",
                        ""busnummer"": ""{x.Busnummer}"",
                        ""postcode"": ""{x.Postcode}"",
                        ""gemeente"": ""{x.Gemeente}"",
                        ""land"": ""{x.Land}""
                    }}"))}
                    ]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": """"
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
