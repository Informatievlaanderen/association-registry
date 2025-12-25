namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
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
using KboMutations.SyncLambda.MagdaSync.SyncKbo;
using AssociationRegistry.OpenTelemetry.Metrics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_Inactief_And_Einddatum
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly DateOnly _einddatum;
    private readonly Mock<INotifier> _notifierMock;

    public With_Inactief_And_Einddatum()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _einddatum = fixture.Create<DateOnly>();

        var verenigingVolgensKbo = fixture.Create<InactieveVereniging>() with
        {
            EindDatum = _einddatum,
        };

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(),
                                      new MagdaSyncGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo),
                                      _notifierMock.Object,
                                      NullLogger<SyncKboCommandHandler>.Instance, Mock.Of<KboSyncMetrics>());

        commandHandler.Handle(
            new CommandEnvelope<SyncKboCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
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
    public void Then_A_VerenigingWerdGestoptInKBO_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .ToArray()
           .Should()
           .ContainInOrder(new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_scenario.KboNummer),
                           new VerenigingWerdGestoptInKBO(_einddatum),
                           new SynchronisatieMetKboWasSuccesvol(_scenario.KboNummer))
           .And
           .HaveCount(3);
    }
}
