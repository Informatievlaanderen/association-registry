﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

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
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_A_New_Adres
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AdresVolgensKbo _newAdres;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_New_Adres()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _newAdres = fixture.Create<AdresVolgensKbo>();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;
        verenigingVolgensKbo.Adres = _newAdres;

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
    public void Then_A_MaatschappelijkeZetelWerdOvergenomenUitKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.Equals(
                              new MaatschappelijkeZetelWerdOvergenomenUitKbo(
                                  new Registratiedata.Locatie(1,
                                                              Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                                              false,
                                                              string.Empty,
                                                              new Registratiedata.Adres(
                                                                  _newAdres.Straatnaam,
                                                                  _newAdres.Huisnummer,
                                                                  _newAdres.Busnummer,
                                                                  _newAdres.Postcode,
                                                                  _newAdres.Gemeente,
                                                                  _newAdres.Land),
                                                              null)
                              )));
    }
}
