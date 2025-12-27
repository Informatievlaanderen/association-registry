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
using Events.Factories;
using FluentAssertions;
using Integrations.Slack;
using KboMutations.SyncLambda.MagdaSync.SyncKbo;
using AssociationRegistry.OpenTelemetry.Metrics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_A_Contactgegeven_That_Exists_With_Bron_Initiator
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Contactgegeven _existingContactgegeven;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Contactgegeven_That_Exists_With_Bron_Initiator()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _existingContactgegeven = fixture.CreateContactgegevenVolgensType(Contactgegeventype.Email) with
        {
            ContactgegevenId = 1,
            Beschrijving = string.Empty,
        };

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState()
                                                                          .Apply(EventFactory.ContactgegevenWerdToegevoegd(_existingContactgegeven)));

        _notifierMock = new Mock<INotifier>();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;

        verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo
        {
            Email = _existingContactgegeven.Waarde,
            Telefoonnummer = null,
            GSM = null,
            Website = null,
        };

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(),
                                      new MagdaSyncGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo),
                                      _notifierMock.Object,
                                      NullLogger<SyncKboCommandHandler>.Instance, new KboSyncMetrics(new System.Diagnostics.Metrics.Meter("test")));

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
    public void Then_A_ContactgegevenWerdInBeheerGenomenDoorKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdInBeheerGenomenDoorKbo(_existingContactgegeven.ContactgegevenId,
                                                                           Contactgegeventype.Email,
                                                                           ContactgegeventypeVolgensKbo.Email.Waarde,
                                                                           _existingContactgegeven.Waarde)));
    }
}
