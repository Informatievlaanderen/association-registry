namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.
    When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingUitKbo;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;
using Xunit;

public class With_An_VerenigingVolgensKbo_Met_Dubbele_Vertegenwoordigers
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;
    private readonly LoggerFactory _loggerFactory;

    public With_An_VerenigingVolgensKbo_Met_Dubbele_Vertegenwoordigers()
    {
        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandMetadata = fixture.Create<CommandMetadata>();

        var dubbeleVertegenwoordigers = CreateDubbeleVertegenwoordigerVolgensKbos(fixture);

        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>() with
        {
            Adres = null,
            Contactgegevens = null,
            Vertegenwoordigers = dubbeleVertegenwoordigers,
        };

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberFoundServiceMock(_verenigingVolgensKbo),
            new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
            Mock.Of<IDocumentSession>(),
            commandHandlerLogger
        );

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    private static VertegenwoordigerVolgensKbo[] CreateDubbeleVertegenwoordigerVolgensKbos(Fixture fixture)
    {
        var insz = fixture.Create<Insz>();

        var dubbeleVertegenwoordigers = fixture
                                       .CreateMany<VertegenwoordigerVolgensKbo>(3)
                                       .Select(v => v with { Insz = insz })
                                       .ToArray();

        return dubbeleVertegenwoordigers;
    }

    [Fact]
    public void Then_it_only_saves_one_VertegenwoordigerWerdOvergenomenUitKBO_event()
    {
        var eersteVertegenwoordiger = _verenigingVolgensKbo.Vertegenwoordigers.First();
        IEvent[] events =
        [
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.KboNummer,
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum),
            new VertegenwoordigerWerdOvergenomenUitKBO(1, eersteVertegenwoordiger.Insz, eersteVertegenwoordiger.Voornaam, eersteVertegenwoordiger.Achternaam),
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        ];

        _verenigingRepositoryMock.ShouldHaveSavedExact(events);
    }
}
