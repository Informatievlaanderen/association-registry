namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Framework;
using VCodes;
using Xunit;

public class When_Detail_Given_KorteNaamWerdGewijzigd_Fixture : AdminApiFixture
{
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    private readonly CommandMetadata _metadata;

    public When_Detail_Given_KorteNaamWerdGewijzigd_Fixture() : base(nameof(When_Detail_Given_KorteNaamWerdGewijzigd_Fixture))
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        _metadata = fixture.Create<CommandMetadata>() with {ExpectedVersion = null};
    }

    public HttpResponseMessage Response { get; private set; } = null!;

    protected override async Task Given()
    {
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            _metadata);
        await AddEvent(
            VCode,
            KorteNaamWerdGewijzigd,
            _metadata);
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetDetail(VCode);
    }
}

public class Given_KorteNaamWerdGewijzigd : IClassFixture<When_Detail_Given_KorteNaamWerdGewijzigd_Fixture>
{
    private readonly When_Detail_Given_KorteNaamWerdGewijzigd_Fixture _adminApiFixture;

    public Given_KorteNaamWerdGewijzigd(When_Detail_Given_KorteNaamWerdGewijzigd_Fixture adminApiFixture)
    {
        _adminApiFixture = adminApiFixture;
    }

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
                    ""korteNaam"": ""{_adminApiFixture.KorteNaamWerdGewijzigd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_adminApiFixture.VerenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""kboNummer"": ""{_adminApiFixture.VerenigingWerdGeregistreerd.KboNummer}"",
                    ""startdatum"": ""{_adminApiFixture.VerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactInfoLijst"": [{string.Join(',', _adminApiFixture.VerenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}""
                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _adminApiFixture.VerenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        {(x.Hoofdlocatie ? $"\"hoofdlocatie\": {x.Hoofdlocatie.ToString().ToLower()}," : string.Empty)}
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
