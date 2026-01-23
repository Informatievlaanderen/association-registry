namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Xunit;

public class With_A_Minimumleeftijd
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private const int NieuweMinimumleeftijd = 1;

    public With_A_Minimumleeftijd()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            Doelgroep: Doelgroep.Create(NieuweMinimumleeftijd, maximumleeftijd: null)
        );

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(
            Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object
        );

        commandHandler
            .Handle(
                new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
                _aggregateSessionMock,
                CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_DoelgroepWerdGewijzigd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(
            new DoelgroepWerdGewijzigd(
                new Registratiedata.Doelgroep(NieuweMinimumleeftijd, Doelgroep.StandaardMaximumleeftijd)
            )
        );
    }
}
