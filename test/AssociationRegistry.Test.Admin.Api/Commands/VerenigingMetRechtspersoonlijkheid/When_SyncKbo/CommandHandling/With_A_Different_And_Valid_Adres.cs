﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

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
public class With_A_Different_And_Valid_Adres
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario _scenario;
    private readonly AdresVolgensKbo _newAdres;
    private readonly Mock<INotifier> _notifierMock;

    public With_A_Different_And_Valid_Adres()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario();
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
    public void Then_A_MaatschappelijkeZetelWerdGewijzigdInKbo_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainEquivalentOf(
                new MaatschappelijkeZetelWerdGewijzigdInKbo(
                    new Registratiedata.Locatie(LocatieId: 1,
                                                Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                                IsPrimair: false,
                                                string.Empty,
                                                new Registratiedata.Adres(
                                                    _newAdres.Straatnaam,
                                                    _newAdres.Huisnummer,
                                                    _newAdres.Busnummer,
                                                    _newAdres.Postcode,
                                                    _newAdres.Gemeente,
                                                    _newAdres.Land),
                                                AdresId: null)
                ));
    }
}
