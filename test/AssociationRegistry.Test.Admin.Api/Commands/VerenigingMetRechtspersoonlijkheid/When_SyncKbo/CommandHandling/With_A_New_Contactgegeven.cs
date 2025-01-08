namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Framework.Fakes;
using Kbo;
using KboSyncLambda.SyncKbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_New_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly string _newEmail;
    private readonly string _newWebsite;
    private readonly string _newGSM;
    private readonly string _newTelefoon;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_New_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _newEmail = fixture.CreateContactgegevenVolgensType(ContactgegeventypeVolgensKbo.Email.Contactgegeventype).Waarde;
        _newWebsite = fixture.CreateContactgegevenVolgensType(ContactgegeventypeVolgensKbo.Website.Contactgegeventype).Waarde;
        _newGSM = fixture.CreateContactgegevenVolgensType(ContactgegeventypeVolgensKbo.GSM.Contactgegeventype).Waarde;
        _newTelefoon = fixture.CreateContactgegevenVolgensType(ContactgegeventypeVolgensKbo.Telefoon.Contactgegeventype).Waarde;

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;

        verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo
        {
            Email = _newEmail,
            Telefoonnummer = _newTelefoon,
            Website = _newWebsite,
            GSM = _newGSM,
        };

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
    public void Then_A_ContactgegevenWerdOvergenomenUitKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdOvergenomenUitKBO(1, Contactgegeventype.Email,
                                                                      ContactgegeventypeVolgensKbo.Email.Waarde, _newEmail)))
           .And
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdOvergenomenUitKBO(2, Contactgegeventype.Website,
                                                                      ContactgegeventypeVolgensKbo.Website.Waarde, _newWebsite)))
           .And
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdOvergenomenUitKBO(3, Contactgegeventype.Telefoon,
                                                                      ContactgegeventypeVolgensKbo.Telefoon.Waarde, _newTelefoon)))
           .And
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdOvergenomenUitKBO(4, Contactgegeventype.Telefoon,
                                                                      ContactgegeventypeVolgensKbo.GSM.Waarde, _newGSM)));
    }
}
