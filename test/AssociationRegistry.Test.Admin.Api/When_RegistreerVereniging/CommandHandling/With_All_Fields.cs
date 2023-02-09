namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Admin.Api.Constants;
using Events;
using AssociationRegistry.Framework;
using INSZ;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_Scenario>>
{
    private const string Naam = "naam1";
    private const string KorteNaam = "korte naam";
    private const string KorteBeschrijving = "korte beschrijving";
    private const string KboNummber = "0123456749";

    private static readonly RegistreerVerenigingCommand.ContactInfo ContactInfo = new("Algemeen", "info@dummy.com", "1234567890", "www.test-website.be", "@test");
    private static readonly RegistreerVerenigingCommand.Locatie Locatie = new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten);
    private static readonly RegistreerVerenigingCommand.ContactInfo VertegenwoordigerContactInfo = new("History", "conan@barbarian.history.com", "0918372645", "www.conan-the-destroyer.history.com", "#ConanTheBarbarian");
    private static readonly RegistreerVerenigingCommand.Vertegenwoordiger Vertegenwoordiger = new(InszTestSet.Insz1_WithCharacters, true, "Conan", "Barbarian, Destroyer", new[] { VertegenwoordigerContactInfo });

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly DateOnly _fromDateTime;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly MagdaPersoon _magdaPersoon;

    public With_All_Fields(CommandHandlerScenarioFixture<Empty_Commandhandler_Scenario> classFixture)
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
            new[] { ContactInfo },
            new[] { Locatie },
            vertegenwoordigers);

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
                        InszTestSet.Insz1,
                        Vertegenwoordiger.PrimairContactpersoon,
                        Vertegenwoordiger.Roepnaam,
                        Vertegenwoordiger.Rol,
                        _magdaPersoon.Voornaam,
                        _magdaPersoon.Achternaam,
                        new[]
                        {
                            new VerenigingWerdGeregistreerd.ContactInfo(
                                VertegenwoordigerContactInfo.Contactnaam,
                                VertegenwoordigerContactInfo.Email,
                                VertegenwoordigerContactInfo.Telefoon,
                                VertegenwoordigerContactInfo.Website,
                                VertegenwoordigerContactInfo.SocialMedia),
                        }),
                }));
    }
}
