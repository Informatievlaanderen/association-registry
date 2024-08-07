﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using Acties.SyncKbo;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Notifications.Messages;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_No_Changes_ReedsIngeschreven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEnIngeschrevenScenario _scenario;
    private readonly Mock<INotifier> _notifierMock;
    private readonly Mock<IMagdaRegistreerInschrijvingService> _magdaRegistreerInschrijvingServiceMock;
    private readonly SyncKboCommand _command;

    public With_No_Changes_ReedsIngeschreven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEnIngeschrevenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _command = new SyncKboCommand(KboNummer.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer));
        var commandMetadata = fixture.Create<CommandMetadata>();

        _magdaRegistreerInschrijvingServiceMock = new Mock<IMagdaRegistreerInschrijvingService>();

        var commandHandler = new SyncKboCommandHandler(_magdaRegistreerInschrijvingServiceMock.Object, new MagdaGeefVerenigingNumberFoundServiceMock(
                                                           _scenario.VerenigingVolgensKbo
                                                       ),
                                                       _notifierMock.Object,
                                                       NullLogger<SyncKboCommandHandler>.Instance);

        commandHandler.Handle(
            new CommandEnvelope<SyncKboCommand>(_command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Vereniging_Is_IsGeregistreerdBijMagda()
    {
        _magdaRegistreerInschrijvingServiceMock
           .Verify(
                service => service.RegistreerInschrijving(
                    _command.KboNummer,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public void Then_No_VerenigingWerdIngeschrevenOpWijzigingenUitKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .NotContain(e => e.GetType() == typeof(VerenigingWerdIngeschrevenOpWijzigingenUitKbo));
    }
}
