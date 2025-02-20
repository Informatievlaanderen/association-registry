namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Framework.Fakes;
using Kbo;
using KboSyncLambda.SyncKbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Notifications.Messages;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

[UnitTest]
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
    public async Task Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        try { await _action(); }
        catch
        {
            // ignored
        }

        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.KboNummer);
    }

    [Fact]
    public async Task Then_One_Notification_Is_Send()
    {
        try { await _action(); }
        catch
        {
            // ignored
        }

        _notifierMock.Verify(notifier => notifier.Notify(
                                 It.Is<KboSynchronisatieMisluktMessage>(
                                     message => message.Value == new KboSynchronisatieMisluktMessage(_scenario.KboNummer).Value)));
    }

    [Fact]
    public async Task Then_No_Event_Is_Saved()
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
