﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.KboSyncLambda.SyncKbo;
using AssociationRegistry.Notifications;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

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

        var commandHandler = new SyncKboCommandHandler(_magdaRegistreerInschrijvingServiceMock.Object,
                                                       new MagdaGeefVerenigingNumberFoundServiceMock(
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
                expression: service => service.RegistreerInschrijving(
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
