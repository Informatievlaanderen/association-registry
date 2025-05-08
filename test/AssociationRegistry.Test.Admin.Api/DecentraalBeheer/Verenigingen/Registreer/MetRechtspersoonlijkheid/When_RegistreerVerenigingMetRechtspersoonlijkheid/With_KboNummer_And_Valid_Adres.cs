namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;
using Polly;

using With_Kbo_Nummer_For_Supported_Rechtsvorm;
using Xunit;
using Xunit.Abstractions;

public class RegistreerForValidAdresSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerForValidAdresSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0407622011")
    {
    }
}

public class With_KboNummer_And_Valid_Adres : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerForValidAdresSetup>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public With_KboNummer_And_Valid_Adres(
        EventsInDbScenariosFixture fixture,
        RegistreerForValidAdresSetup registreerForValidAdresSetup,
        ITestOutputHelper testOutputHelper) : base(fixture, registreerForValidAdresSetup)
    {
        _testOutputHelper = testOutputHelper;
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
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("V.L.K.");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(year: 1989, month: 10, day: 03));
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.VZW.Code);
        }

        var policyResult = await Policy.Handle<Exception>()
                                       .RetryAsync(retryCount: 5,
                                                   onRetryAsync: async (_, i) => await Task.Delay(TimeSpan.FromSeconds(i * 1)))
                                       .ExecuteAndCaptureAsync(async () =>
                                        {
                                            var fetchStreamAsync =
                                                await session.Events.FetchStreamAsync(
                                                    verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

                                            var maatschappelijkeZetelWerdOvergenomenUitKbo = fetchStreamAsync
                                               .Should().ContainSingle(
                                                    e => e.Data.GetType() ==
                                                         typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo)).Subject;

                                            maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Should().BeEquivalentTo(
                                                new MaatschappelijkeZetelWerdOvergenomenUitKbo(
                                                    new Registratiedata.Locatie(LocatieId: 1,
                                                                                Locatietype: "Maatschappelijke zetel volgens KBO",
                                                                                IsPrimair: false, Naam: "",
                                                                                new Registratiedata.Adres(
                                                                                    Straatnaam: "Koningsstraat",
                                                                                    Huisnummer: "217",
                                                                                    string.Empty,
                                                                                    Postcode: "1210",
                                                                                    Gemeente: "Sint-Joost-ten-Node",
                                                                                    Land: "België"),
                                                                                AdresId: null)));
                                        });

        policyResult.FinalException.Should().BeNull();
    }
}
