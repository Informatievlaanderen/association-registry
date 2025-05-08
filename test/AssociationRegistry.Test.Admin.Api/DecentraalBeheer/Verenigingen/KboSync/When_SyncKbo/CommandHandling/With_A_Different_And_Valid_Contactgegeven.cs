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

public class With_A_Different_And_Valid_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario _scenario;
    private readonly string _newContactgegevenWaarde;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Different_And_Valid_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();

        _newContactgegevenWaarde = fixture.CreateContactgegevenVolgensType(_scenario.ContactgegevenWerdOvergenomenUitKBO.Contactgegeventype)
                                          .Waarde;

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;

        verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo
        {
            Email = verenigingVolgensKbo.Contactgegevens.Email is null ? null : _newContactgegevenWaarde,
            Website = verenigingVolgensKbo.Contactgegevens.Website is null ? null : _newContactgegevenWaarde,
            Telefoonnummer = verenigingVolgensKbo.Contactgegevens.Telefoonnummer is null ? null : _newContactgegevenWaarde,
            GSM = verenigingVolgensKbo.Contactgegevens.GSM is null ? null : _newContactgegevenWaarde,
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
    public void Then_A_NaamWerdGewijzigdInKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(new ContactgegevenWerdGewijzigdInKbo(
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId,
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.Contactgegeventype,
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo,
                                            _newContactgegevenWaarde)));
    }
}
