namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Admin.Api.Constants;
using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using AutoFixture;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase>>
{
    private const string Naam = "naam1";
    private const string KorteNaam = "korte naam";
    private const string KorteBeschrijving = "korte beschrijving";
    private const string KboNummer = "0123456749";
    private const string HoofdactiviteitCode = "KECU";

    private static readonly Contactgegeven Contactgegeven = Contactgegeven.Create(ContactgegevenType.Email, "info@dummy.com", "the email", true);
    private static readonly Locatie Locatie = new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten);
    private static readonly Contactgegeven VertegenwoordigerContactgegeven = Contactgegeven.Create(ContactgegevenType.Email, "conan@barbarian.history.com", "Historie", true);
    private static readonly Vertegenwoordiger Vertegenwoordiger = Vertegenwoordiger.Create(Insz.Create(InszTestSet.Insz1_WithCharacters), true, "Conan", "Barbarian, Destroyer", new[] { VertegenwoordigerContactgegeven });

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly DateOnly _dateInThePast;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly MagdaPersoon _magdaPersoon;

    public With_All_Fields(CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();
        Mock<IMagdaFacade> magdaFacade = new();

        var fixture = new Fixture().CustomizeAll();

        var today = fixture.Create<DateOnly>();
        _dateInThePast = today.AddDays(-3);

        var clock = new ClockStub(today);

        var vertegenwoordigers = new[] { Vertegenwoordiger };
        var command = new RegistreerVerenigingCommand(
            VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            Startdatum.Create(_dateInThePast),
            AssociationRegistry.Vereniging.KboNummer.Create(KboNummer),
            new[] { Contactgegeven },
            new[] { Locatie },
            vertegenwoordigers,
            new[] { HoofdactiviteitVerenigingsloket.Create(HoofdactiviteitCode) });

        _magdaPersoon = new MagdaPersoon
        {
            Insz = Insz.Create(Vertegenwoordiger.Insz),
            Voornaam = "Thor",
            Achternaam = "Odinson",
            IsOverleden = false,
        };
        magdaFacade
            .Setup(facade => facade.GetByInsz(It.Is<Insz>(insz => string.Equals(insz.ToString(), InszTestSet.Insz1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_magdaPersoon);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, magdaFacade.Object, new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                Naam,
                KorteNaam,
                KorteBeschrijving,
                _dateInThePast,
                KboNummer,
                new[]
                {
                    new VerenigingWerdGeregistreerd.Contactgegeven(
                        1,
                        ContactgegevenType.Email,
                        Contactgegeven.Waarde,
                        Contactgegeven.Omschrijving,
                        Contactgegeven.IsPrimair
                    ),
                },
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        Locatie.Naam,
                        Locatie.Straatnaam,
                        Locatie.Huisnummer,
                        Locatie.Busnummer,
                        Locatie.Postcode,
                        Locatie.Gemeente,
                        Locatie.Land,
                        Locatie.Hoofdlocatie,
                        Locatie.Locatietype),
                },
                new[]
                {
                    new VerenigingWerdGeregistreerd.Vertegenwoordiger(
                        InszTestSet.Insz1,
                        Vertegenwoordiger.PrimairContactpersoon,
                        Vertegenwoordiger.Roepnaam,
                        Vertegenwoordiger.Rol,
                        _magdaPersoon.Voornaam,
                        _magdaPersoon.Achternaam,
                        new[]
                        {
                            new VerenigingWerdGeregistreerd.Contactgegeven(
                                1,
                                ContactgegevenType.Email,
                                VertegenwoordigerContactgegeven.Waarde,
                                VertegenwoordigerContactgegeven.Omschrijving,
                                VertegenwoordigerContactgegeven.IsPrimair),
                        }),
                },
                new[]
                {
                    new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(HoofdactiviteitCode, HoofdactiviteitVerenigingsloket.All().Single(a => a.Code == HoofdactiviteitCode).Beschrijving),
                }));
    }
}
