﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly CommandMetadata _metadata;
    private readonly V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V044VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelVolgensKbo;

        _vCode = _scenario.VCode;
        _metadata = _scenario.Metadata;

        _adminApiClient = fixture.DefaultClient;
        _response = _adminApiClient.GetHistoriek(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, long.MaxValue))
          .StatusCode
          .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenis_for_moeder()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/historiek-vereniging-context.json"}"",
                ""vCode"": ""{_vCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""VerenigingMetRechtspersoonlijkheidWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
{{
                        ""beschrijving"": ""De locatie met type ‘Maatschappelijke Zetel volgens KBO' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""MaatschappelijkeZetelWerdOvergenomenUitKbo"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
{{
                        ""beschrijving"": ""Maatschappelijke Zetel volgens KBO werd gewijzigd."",
                        ""gebeurtenis"":""MaatschappelijkeZetelVolgensKBOWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
