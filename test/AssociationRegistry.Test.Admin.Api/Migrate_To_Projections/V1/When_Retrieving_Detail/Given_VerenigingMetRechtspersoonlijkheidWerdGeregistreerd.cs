namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail;

using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;
        _adminApiClient = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue))
          .StatusCode
          .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Straatnaam,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Huisnummer,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Busnummer,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Land,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.IsPrimair,
                                   Bron.KBO, _scenario.VCode)
                      .WithContactgegeven(
                           _scenario.EmailWerdOvergenomenUitKBO.ContactgegevenId,
                           _scenario.EmailWerdOvergenomenUitKBO.Bron,
                           _scenario.EmailWerdOvergenomenUitKBO.Contactgegeventype,
                           _scenario.EmailWerdOvergenomenUitKBO.Waarde, _scenario.VCode,
                           _scenario.EmailWerdGewijzigd.Beschrijving,
                           _scenario.EmailWerdGewijzigd.IsPrimair)
                      .WithContactgegeven(_scenario.WebsiteWerdOvergenomenUitKBO.ContactgegevenId,
                                          _scenario.WebsiteWerdOvergenomenUitKBO.Bron,
                                          _scenario.WebsiteWerdOvergenomenUitKBO.Contactgegeventype,
                                          _scenario.WebsiteWerdOvergenomenUitKBO.Waarde, _scenario.VCode)
                      .WithContactgegeven(_scenario.TelefoonWerdOvergenomenUitKBO.ContactgegevenId,
                                          _scenario.TelefoonWerdOvergenomenUitKBO.Bron,
                                          _scenario.TelefoonWerdOvergenomenUitKBO.Contactgegeventype,
                                          _scenario.TelefoonWerdOvergenomenUitKBO.Waarde, _scenario.VCode)
                      .WithContactgegeven(_scenario.GSMWerdOvergenomenUitKBO.ContactgegevenId,
                                          _scenario.GSMWerdOvergenomenUitKBO.Bron,
                                          _scenario.GSMWerdOvergenomenUitKBO.Contactgegeventype,
                                          _scenario.GSMWerdOvergenomenUitKBO.Waarde, _scenario.VCode)
                      .WithVertegenwoordiger(_scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId,
                                             _scenario.VertegenwoordigerWerdOvergenomenUitKBO.Voornaam,
                                             _scenario.VertegenwoordigerWerdOvergenomenUitKBO.Achternaam,
                                             string.Empty,
                                             string.Empty, _scenario.VertegenwoordigerWerdOvergenomenUitKBO.Insz, string.Empty,
                                             string.Empty, string.Empty, string.Empty, isPrimair: false, Bron.KBO, _scenario.VCode);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        response.Headers.ETag.Should().NotBeNull();

        var etag = response.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
