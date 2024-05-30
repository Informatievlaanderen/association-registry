﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_VerenigingVolgensKbo_Contactgegevens
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;
    private readonly LoggerFactory _loggerFactory;

    public With_VerenigingVolgensKbo_Contactgegevens()
    {
        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();
        _verenigingVolgensKbo.Contactgegevens = fixture.Create<ContactgegevensVolgensKbo>();

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler();

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata),
                   _verenigingRepositoryMock,
                   _vCodeService,
                   new MagdaGeefVerenigingNumberFoundServiceMock(_verenigingVolgensKbo),
                   new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
                   commandHandlerLogger, CancellationToken.None)
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
            new ContactgegevenWerdOvergenomenUitKBO(
                ContactgegevenId: 1,
                Contactgegeventype.Email.Waarde,
                ContactgegeventypeVolgensKbo.Email,
                _verenigingVolgensKbo.Contactgegevens.Email!
            ),
            new ContactgegevenWerdOvergenomenUitKBO(
                ContactgegevenId: 2,
                Contactgegeventype.Website.Waarde,
                ContactgegeventypeVolgensKbo.Website,
                _verenigingVolgensKbo.Contactgegevens.Website!
            ),
            new ContactgegevenWerdOvergenomenUitKBO(
                ContactgegevenId: 3,
                Contactgegeventype.Telefoon.Waarde,
                ContactgegeventypeVolgensKbo.Telefoon,
                _verenigingVolgensKbo.Contactgegevens.Telefoonnummer!
            ),
            new ContactgegevenWerdOvergenomenUitKBO(
                ContactgegevenId: 4,
                Contactgegeventype.Telefoon.Waarde,
                ContactgegeventypeVolgensKbo.GSM,
                _verenigingVolgensKbo.Contactgegevens.GSM!
            ),
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        );
    }
}

[UnitTest]
public class With_VerenigingVolgensKbo_No_Contactgegevens
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;
    private readonly LoggerFactory _loggerFactory;

    public With_VerenigingVolgensKbo_No_Contactgegevens()
    {
        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();

        _verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo
        {
            Email = null,
            Telefoonnummer = null,
            Website = null,
            GSM = null,
        };

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler();

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata),
                   _verenigingRepositoryMock,
                   _vCodeService,
                   new MagdaGeefVerenigingNumberFoundServiceMock(_verenigingVolgensKbo),
                   new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
                   commandHandlerLogger, CancellationToken.None)
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
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        );
    }
}
