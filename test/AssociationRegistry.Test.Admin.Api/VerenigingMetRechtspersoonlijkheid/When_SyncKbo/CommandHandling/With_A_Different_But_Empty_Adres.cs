namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

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
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Different_But_Empty_Adres
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario _scenario;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Different_But_Empty_Adres()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;
        verenigingVolgensKbo.Adres = new AdresVolgensKbo();

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(), new MagdaGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo), _notifierMock.Object, NullLogger<SyncKboCommandHandler>.Instance);

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
    public void Then_A_MaatschappelijkeZetelWerdVerwijderdInKbo_Event_Is_Saved()
    {
        var events = _verenigingRepositoryMock
                    .SaveInvocations[0]
                    .Vereniging
                    .UncommittedEvents
                    .Should()
                    .HaveCount(2)
                    .And.Subject.ToArray();

        events[0].Should().BeEquivalentTo(
            new MaatschappelijkeZetelWerdVerwijderdUitKbo(
                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie));

        events[1].Should().BeOfType<SynchronisatieMetKboWasSuccesvol>();
    }
}
