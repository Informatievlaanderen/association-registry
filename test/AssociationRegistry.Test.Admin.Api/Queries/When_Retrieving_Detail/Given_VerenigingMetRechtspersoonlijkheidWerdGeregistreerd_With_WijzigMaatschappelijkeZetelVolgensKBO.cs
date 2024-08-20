<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Formats;
using Framework;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail;

using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO.cs
using Framework.Fixtures;
using Framework.templates;
using Microsoft.Net.Http.Headers;
using System.Net;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO.cs
using Vereniging.Bronnen;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO.cs
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO(
        EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V044VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelVolgensKbo;
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
                                   _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.Naam,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Straatnaam,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Huisnummer,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Busnummer,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                   _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Land,
                                   _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.IsPrimair,
                                   Bron.KBO, _scenario.VCode);

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
