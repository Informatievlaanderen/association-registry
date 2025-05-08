namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using Framework;
using templates;
using Vereniging;

using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced
{
    private readonly V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd;

        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithType(Verenigingstype.Parse(_scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm))
                          .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                          .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam)
                          .WithLocatie(_scenario.VCode,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId.ToString(),
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString(),
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Straatnaam,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Huisnummer,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Busnummer,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Gemeente,
                                       _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Land)
                          .WithContactgegeven(_scenario.VCode,
                                              _scenario.ContactgegevenWerdOvergenomenUitKbo.ContactgegevenId.ToString(),
                                              _scenario.ContactgegevenWerdOvergenomenUitKbo.Contactgegeventype,
                                              _scenario.ContactgegevenWerdGewijzigdInKbo.Waarde)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
