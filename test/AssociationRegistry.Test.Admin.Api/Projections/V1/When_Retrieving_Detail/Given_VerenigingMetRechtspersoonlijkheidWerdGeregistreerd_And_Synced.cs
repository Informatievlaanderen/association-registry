namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using Common.Extensions;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced;
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
                      .WithType(Verenigingstype.Parse(_scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm))
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                      .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam)
                      .WithContactgegeven(_scenario.ContactgegevenWerdOvergenomenUitKbo.ContactgegevenId, Bron.KBO,
                                          _scenario.ContactgegevenWerdOvergenomenUitKbo.Contactgegeventype,
                                          _scenario.ContactgegevenWerdGewijzigdInKbo.Waarde, _scenario.VCode)
                      .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString(),
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Straatnaam,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Huisnummer,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Busnummer,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Gemeente,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Land,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.AdresId.Broncode,
                                   _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.AdresId.Bronwaarde,
                                   isPrimair: false,
                                   Bron.KBO,
                                   _scenario.VCode)
                      .WithStatus(VerenigingStatus.Gestopt)
                      .WithEinddatum(_scenario.VerenigingWerdGestoptInKbo.Einddatum);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        response.Headers.ETag.Should().NotBeNull();

        var etag = response.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().Be("W/\"11\"");
    }
}
