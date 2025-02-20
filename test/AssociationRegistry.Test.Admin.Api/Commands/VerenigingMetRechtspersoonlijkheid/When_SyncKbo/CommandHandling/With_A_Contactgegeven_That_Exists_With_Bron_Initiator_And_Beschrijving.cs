﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using EventFactories;
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
public class With_A_Contactgegeven_That_Exists_With_Bron_Initiator_And_Beschrijving
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Contactgegeven _existingContactgegeven;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Contactgegeven_That_Exists_With_Bron_Initiator_And_Beschrijving()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _existingContactgegeven = fixture.CreateContactgegevenVolgensType(Contactgegeventype.Email) with
        {
            ContactgegevenId = 1,
            Beschrijving = fixture.Create<string>(),
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
    public void Then_A_ContactgegevenWerdInBeheerGenomenDoorKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(
                              new ContactgegevenWerdOvergenomenUitKBO(2, Contactgegeventype.Email,
                                                                      ContactgegeventypeVolgensKbo.Email.Waarde,
                                                                      _existingContactgegeven.Waarde)));
    }
}
