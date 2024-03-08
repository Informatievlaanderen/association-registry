namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using Acties.SyncKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Kbo;
using Moq;
using Notifications;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Different_And_Invalid_Adres
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario _scenario;
    private readonly AdresVolgensKbo _newAdres;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Different_And_Invalid_Adres()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _newAdres = fixture.Create<AdresVolgensKbo>();
        _newAdres.Huisnummer = string.Empty;

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;
        verenigingVolgensKbo.Adres = _newAdres;

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new SyncKboCommandHandler(new MagdaGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo), _notifierMock.Object);

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
                    .HaveCount(3)
                    .And.Subject.ToArray();

        events[0].Should().BeEquivalentTo(
            new MaatschappelijkeZetelWerdVerwijderdUitKbo(
                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie));

        events[1].Should().BeEquivalentTo(new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
                                              _newAdres.Straatnaam,
                                              _newAdres.Huisnummer,
                                              _newAdres.Busnummer,
                                              _newAdres.Postcode,
                                              _newAdres.Gemeente,
                                              _newAdres.Land));

        events[2].Should().BeOfType<SynchronisatieMetKboWasSuccesvol>();
    }
}
