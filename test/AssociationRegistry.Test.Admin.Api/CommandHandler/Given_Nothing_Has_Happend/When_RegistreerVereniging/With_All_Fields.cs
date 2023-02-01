namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_Nothing_Has_Happend.When_RegistreerVereniging;

using AssociationRegistry.Admin.Api.Constants;
using Events;
using AssociationRegistry.Framework;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using INSZ;
using Magda;
using Moq;
using Scenarios;
using Vertegenwoordigers;
using Xunit;

public class With_All_Fields : IClassFixture<Given_A_Scenario_CommandHandlerFixture<EmptyScenario>>
{
    private const string Naam = "naam1";
    private const string KorteNaam = "korte naam";
    private const string KorteBeschrijving = "korte beschrijving";
    private const string KboNummber = "0123456749";

    private static readonly RegistreerVerenigingCommand.ContactInfo ContactInfo = new("Algemeen", "info@dummy.com", "1234567890", "www.test-website.be", "@test");
    private static readonly RegistreerVerenigingCommand.Locatie Locatie = new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten);
    private static readonly RegistreerVerenigingCommand.Vertegenwoordiger Vertegenwoordiger = new("01.13.15-001.49", true, "Conan", "Barbarian, Destroyer");

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly DateOnly _fromDateTime;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly Vertegenwoordiger _magdaVertegenwoordiger;

    public With_All_Fields(Given_A_Scenario_CommandHandlerFixture<EmptyScenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();
        Mock<IMagdaFacade> magdaFacade = new();

        var fixture = new Fixture();

        var today = fixture.Create<DateTime>();
        _fromDateTime = DateOnly.FromDateTime(today.AddDays(-3));

        var clock = new ClockStub(today);

        var vertegenwoordigers = new[] { Vertegenwoordiger };
        var command = new RegistreerVerenigingCommand(
            Naam,
            KorteNaam,
            KorteBeschrijving,
            _fromDateTime,
            KboNummber,
            new[] { ContactInfo, },
            new[] { Locatie, },
            vertegenwoordigers);

        _magdaVertegenwoordiger = Vertegenwoordigers.Vertegenwoordiger.Create(
            Insz.Create(Vertegenwoordiger.Insz),
            Vertegenwoordiger.PrimairContactpersoon,
            Vertegenwoordiger.Roepnaam,
            Vertegenwoordiger.Rol,
            "Thor",
            "Odinson");
        var vertegenwoordigersLijst = VertegenwoordigersLijst.Create(
            new[]
            {
                _magdaVertegenwoordiger,
            });
        magdaFacade
            .Setup(facade => facade.GetVertegenwoordigers(vertegenwoordigers, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vertegenwoordigersLijst);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, magdaFacade.Object, clock);

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
                _fromDateTime,
                KboNummber,
                new[]
                {
                    new VerenigingWerdGeregistreerd.ContactInfo(
                        ContactInfo.Contactnaam,
                        ContactInfo.Email,
                        ContactInfo.Telefoon,
                        ContactInfo.Website,
                        ContactInfo.SocialMedia),
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
                        "01131500149",
                        _magdaVertegenwoordiger.PrimairContactpersoon,
                        _magdaVertegenwoordiger.Roepnaam,
                        _magdaVertegenwoordiger.Rol,
                        _magdaVertegenwoordiger.Voornaam,
                        _magdaVertegenwoordiger.Achternaam),
                }));
    }
}
