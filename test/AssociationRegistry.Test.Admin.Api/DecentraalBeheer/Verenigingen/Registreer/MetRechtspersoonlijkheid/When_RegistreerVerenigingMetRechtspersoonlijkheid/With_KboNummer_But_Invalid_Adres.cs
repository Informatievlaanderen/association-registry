namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;

using With_Kbo_Nummer_For_Supported_Rechtsvorm;
using Xunit;

public class RegistreerForInvalidAdresSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerForInvalidAdresSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0408498573")
    {
    }
}

public class With_KboNummer_But_Invalid_Adres : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerForInvalidAdresSetup>
{
    public With_KboNummer_But_Invalid_Adres(
        EventsInDbScenariosFixture fixture,
        RegistreerForInvalidAdresSetup registreerForInvalidAdresSetup) : base(fixture, registreerForInvalidAdresSetup)
    {
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = session
                                                                 .Events
                                                                 .QueryRawEventDataOnly<
                                                                      VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                                                 .Should().ContainSingle(
                                                                      e => e.KboNummer == RegistreerVerenigingMetRechtspersoonlijkheidSetup
                                                                                         .UitKboRequest.KboNummer).Subject;

        using (new AssertionScope())
        {
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam.Should().Be("Boite A Musique");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(year: 1933, month: 01, day: 01));
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.VZW.Code);
        }

        var fetchStreamAsync = await session.Events.FetchStreamAsync(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

        var maatschappelijkeZetelWerdOvergenomenUitKbo = fetchStreamAsync
                                                        .Should().ContainSingle(
                                                             e => e.Data.GetType() ==
                                                                  typeof(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo)).Subject;

        maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Should().BeEquivalentTo(
            new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
                Straatnaam: "",
                Huisnummer: "",
                string.Empty,
                Postcode: "1000",
                Gemeente: "Brussel",
                Land: "België")
        );
    }
}
