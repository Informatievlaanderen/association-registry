namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.OpenTelemetry.Metrics;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Integrations.Slack;
using KboMutations.SyncLambda.MagdaSync.SyncKbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_A_Different_And_Invalid_Adres
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly VerenigingsStateQueriesMock _verenigingStateQueryServiceMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario _scenario;
    private readonly AdresVolgensKbo _newAdres;
    private readonly Mock<INotifier> _notifierMock;
    private readonly VerenigingsStateQueriesMock _verenigingsStateQueriesMock;

    public With_A_Different_And_Invalid_Adres()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario();
        var verenigingState = _scenario.GetVerenigingState();
        _aggregateSessionMock = new AggregateSessionMock(verenigingState);
        _verenigingStateQueryServiceMock = new VerenigingsStateQueriesMock(verenigingState);
        _verenigingsStateQueriesMock = new VerenigingsStateQueriesMock(_scenario.GetVerenigingState());
        _notifierMock = new Mock<INotifier>();

        var fixture = new Fixture().CustomizeAdminApi();
        _newAdres = fixture.Create<AdresVolgensKbo>();
        _newAdres.Huisnummer = string.Empty;

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;
        verenigingVolgensKbo.Adres = _newAdres;

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new SyncKboCommandHandler(
            Mock.Of<IMagdaRegistreerInschrijvingService>(),
            new MagdaSyncGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo),
            _notifierMock.Object,
            NullLogger<SyncKboCommandHandler>.Instance,
            new KboSyncMetrics(new System.Diagnostics.Metrics.Meter("test"))
        );

        commandHandler
            .Handle(
                new CommandEnvelope<SyncKboCommand>(command, commandMetadata),
                _aggregateSessionMock,
                _verenigingsStateQueriesMock
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.KboNummer);
    }

    [Fact]
    public void Then_No_Notification_Is_Send()
    {
        _notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Then_A_MaatschappelijkeZetelWerdVerwijderdInKbo_Event_Is_Saved()
    {
        var events = _aggregateSessionMock
            .SaveInvocations[0]
            .Vereniging.UncommittedEvents.Should()
            .ContainSingle(e =>
                e as MaatschappelijkeZetelWerdVerwijderdUitKbo
                == new MaatschappelijkeZetelWerdVerwijderdUitKbo(
                    _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie
                )
            )
            .And.ContainSingle(e =>
                e as MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo
                == new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
                    _newAdres.Straatnaam,
                    _newAdres.Huisnummer,
                    _newAdres.Busnummer,
                    _newAdres.Postcode,
                    _newAdres.Gemeente,
                    _newAdres.Land
                )
            );
    }
}
