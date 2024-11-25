namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using Framework;
using Framework.Fakes;
using Kbo;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_VerenigingVolgensKbo_Invalid_Contactgegevens
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;
    private readonly LoggerFactory _loggerFactory;

    public With_VerenigingVolgensKbo_Invalid_Contactgegevens()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();
        _loggerFactory = new LoggerFactory();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();

        _verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo
        {
            Email = "bla",
            Website = fixture.Create<string>(),
            Telefoonnummer = fixture.Create<string>(),
            GSM = fixture.Create<string>(),
        };

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberFoundServiceMock(
                _verenigingVolgensKbo
            ),
            new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
            commandHandlerLogger
        );

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.KboNummer,
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                Contactgegeventype.Email.Waarde,
                ContactgegeventypeVolgensKbo.Email.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Email!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                Contactgegeventype.Website.Waarde,
                ContactgegeventypeVolgensKbo.Website.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Website!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                Contactgegeventype.Telefoon.Waarde,
                ContactgegeventypeVolgensKbo.Telefoon.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Telefoonnummer!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                Contactgegeventype.Telefoon.Waarde,
                ContactgegeventypeVolgensKbo.GSM.Waarde,
                _verenigingVolgensKbo.Contactgegevens.GSM!
            ),
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        );
    }
}
