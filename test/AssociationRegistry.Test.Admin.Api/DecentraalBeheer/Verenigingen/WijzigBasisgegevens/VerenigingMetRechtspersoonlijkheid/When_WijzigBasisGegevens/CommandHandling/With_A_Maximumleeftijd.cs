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

public class With_A_Maximumleeftijd
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private const int NieuweMaximumleeftijd = 142;

    public With_A_Maximumleeftijd()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            Doelgroep: Doelgroep.Create(minimumleeftijd: null, NieuweMaximumleeftijd));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object);

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_DoelgroepWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new DoelgroepWerdGewijzigd(
                new Registratiedata.Doelgroep(
                    Doelgroep.StandaardMinimumleeftijd,
                    NieuweMaximumleeftijd))
        );
    }
}
