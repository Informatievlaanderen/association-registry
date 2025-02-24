namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

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
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Different_StartDatum
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly DateOnly _newStartdatum;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Different_StartDatum()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _newStartdatum = fixture.Create<DateOnly>();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;
        verenigingVolgensKbo.Startdatum = _newStartdatum;

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(),
                                      new MagdaGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo),
                                      _notifierMock.Object,
                                      NullLogger<SyncKboCommandHandler>.Instance);

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
    public void Then_A_NaamWerdGewijzigdInKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(new StartdatumWerdGewijzigdInKbo(_newStartdatum)));
    }
}
