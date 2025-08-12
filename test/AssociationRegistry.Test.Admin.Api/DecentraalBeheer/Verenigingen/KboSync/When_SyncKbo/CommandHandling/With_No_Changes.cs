namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.CommandHandling.KboSyncLambda.SyncKbo;
using AssociationRegistry.CommandHandling.KboSyncLambda.SyncKbo.Messages;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Integrations.Slack;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_No_Changes
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;
    private readonly Mock<INotifier> _notifierMock;
    private readonly Mock<IMagdaRegistreerInschrijvingService> _magdaRegistreerInschrijvingServiceMock;
    private readonly SyncKboCommand _command;

    public With_No_Changes()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();

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
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.KboNummer);
    }

    [Fact]
    public void Then_No_Notification_Is_Send()
    {
        _notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Then_Only_A_SynchronisatieMetKboWasSuccesvol_And_VerenigingWerdIngeschrevenOpWijzigingenUitKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .HaveCount(2)
           .And
           .ContainSingle(e => e.GetType() == typeof(SynchronisatieMetKboWasSuccesvol))
           .And
           .ContainSingle(e => e.GetType() == typeof(VerenigingWerdIngeschrevenOpWijzigingenUitKbo));
    }
}

public class With_FailureResultFromMagda
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Mock<INotifier> _notifierMock;
    private readonly Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;
    private readonly Func<Task<CommandResult>> _action;

    public With_FailureResultFromMagda()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();

        _magdaGeefVerenigingService
           .Setup(s => s.GeefSyncVereniging(It.IsAny<KboNummer>(), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(VerenigingVolgensKboResult.GeenGeldigeVereniging);

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new SyncKboCommand(KboNummer.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer));
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(),
                                                       _magdaGeefVerenigingService.Object,
                                                       _notifierMock.Object,
                                                       NullLogger<SyncKboCommandHandler>.Instance);

        _action = async () => await commandHandler.Handle(
            new CommandEnvelope<SyncKboCommand>(command, commandMetadata),
            _verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_One_Notification_Is_Send()
    {
        try { await _action(); }
        catch
        {
            // ignored
        }

        _notifierMock.Verify(notifier => notifier.Notify(
                                 It.Is<KboSynchronisatieMisluktNotification>(
                                     message => message.Value == new KboSynchronisatieMisluktNotification(_scenario.KboNummer).Value)));
    }

    [Fact]
    public async ValueTask Then_No_Event_Is_Saved()
    {
        try { await _action(); }
        catch
        {
            // ignored
        }

        _verenigingRepositoryMock.SaveInvocations.Should().BeEmpty();
    }

    [Fact]
    public void Then_It_Throws()
    {
        _action.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}
