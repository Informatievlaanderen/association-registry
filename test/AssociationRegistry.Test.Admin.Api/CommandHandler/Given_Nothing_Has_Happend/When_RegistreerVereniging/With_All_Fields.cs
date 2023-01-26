namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_Nothing_Has_Happend.When_RegistreerVereniging;

using AssociationRegistry.Admin.Api.Constants;
using Events;
using AssociationRegistry.Framework;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Scenarios;
using Xunit;

public class With_All_Fields : IClassFixture<Given_A_Scenario_CommandHandlerFixture<EmptyScenario>>
{
    private const string Naam = "naam1";
    private const string KorteNaam = "korte naam";
    private const string KorteBeschrijving = "korte beschrijving";
    private const string KboNummber = "0123456749";

    private static readonly RegistreerVerenigingCommand.ContactInfo ContactInfo = new("Algemeen", "info@dummy.com", "1234567890", "www.test-website.be", "@test");
    private static readonly RegistreerVerenigingCommand.Locatie Locatie = new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten);

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly DateOnly _fromDateTime;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_All_Fields(Given_A_Scenario_CommandHandlerFixture<EmptyScenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture();

        var today = fixture.Create<DateTime>();
        _fromDateTime = DateOnly.FromDateTime(today.AddDays(-3));

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingCommand(
            Naam,
            KorteNaam,
            KorteBeschrijving,
            _fromDateTime,
            KboNummber,
            new[] { ContactInfo, },
            new[] { Locatie, });
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, clock);

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
                }));
    }
}
