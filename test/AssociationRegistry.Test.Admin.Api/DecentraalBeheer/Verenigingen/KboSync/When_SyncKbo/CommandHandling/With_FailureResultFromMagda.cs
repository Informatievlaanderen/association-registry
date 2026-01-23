namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Integrations.Slack;
using KboMutations.SyncLambda.MagdaSync.SyncKbo;
using KboMutations.SyncLambda.MagdaSync.SyncKbo.Notifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenTelemetry.Metrics;
using Xunit;

public class With_FailureResultFromMagda
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly VerenigingsStateQueriesMock _verenigingStateQueryServiceMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Mock<INotifier> _notifierMock;
    private readonly Mock<IMagdaSyncGeefVerenigingService> _magdaGeefVerenigingService;
    private readonly Func<Task<CommandResult>> _action;

    public With_FailureResultFromMagda()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingState = _scenario.GetVerenigingState();
        _aggregateSessionMock = new AggregateSessionMock(verenigingState);
        _verenigingStateQueryServiceMock = new VerenigingsStateQueriesMock(verenigingState);
        _notifierMock = new Mock<INotifier>();

        _magdaGeefVerenigingService = new Mock<IMagdaSyncGeefVerenigingService>();

        _magdaGeefVerenigingService
            .Setup(s =>
                s.GeefVereniging(
                    It.IsAny<KboNummer>(),
                    It.IsAny<AanroependeFunctie>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(VerenigingVolgensKboResult.GeenGeldigeVereniging);

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new SyncKboCommand(
            KboNummer.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer)
        );
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new SyncKboCommandHandler(
            Mock.Of<IMagdaRegistreerInschrijvingService>(),
            _magdaGeefVerenigingService.Object,
            _notifierMock.Object,
            NullLogger<SyncKboCommandHandler>.Instance,
            new KboSyncMetrics(new System.Diagnostics.Metrics.Meter("test"))
        );

        _action = async () =>
            await commandHandler.Handle(
                new CommandEnvelope<SyncKboCommand>(command, commandMetadata),
                _aggregateSessionMock,
                _verenigingStateQueryServiceMock
            );
    }

    [Fact]
    public async ValueTask Then_One_Notification_Is_Send()
    {
        try
        {
            await _action();
        }
        catch
        {
            // ignored
        }

        _notifierMock.Verify(notifier =>
            notifier.Notify(
                It.Is<KboSynchronisatieMisluktNotification>(message =>
                    message.Value == new KboSynchronisatieMisluktNotification(_scenario.KboNummer).Value
                )
            )
        );
    }

    [Fact]
    public async ValueTask Then_No_Event_Is_Saved()
    {
        try
        {
            await _action();
        }
        catch
        {
            // ignored
        }

        _aggregateSessionMock.SaveInvocations.Should().BeEmpty();
    }

    [Fact]
    public void Then_It_Throws()
    {
        _action.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}
