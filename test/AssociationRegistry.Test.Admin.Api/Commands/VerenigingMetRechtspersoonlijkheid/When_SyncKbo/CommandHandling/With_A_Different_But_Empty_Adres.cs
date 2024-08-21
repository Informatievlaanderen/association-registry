namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using Acties.SyncKbo;
using Events;
using AssociationRegistry.Framework;
using Kbo;
using Notifications;
using Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Framework.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
    public void Then_A_MaatschappelijkeZetelWerdVerwijderdInKbo_Event_Is_Saved()
    {
        var events = _verenigingRepositoryMock
                    .SaveInvocations[0]
                    .Vereniging
                    .UncommittedEvents
                    .Should()
                    .ContainSingle(e => e as MaatschappelijkeZetelWerdVerwijderdUitKbo == new MaatschappelijkeZetelWerdVerwijderdUitKbo(
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie));
    }
}
