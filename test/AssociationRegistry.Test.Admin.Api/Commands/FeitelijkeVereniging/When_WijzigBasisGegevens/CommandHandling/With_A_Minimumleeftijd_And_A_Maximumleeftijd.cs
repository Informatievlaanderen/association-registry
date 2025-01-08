namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework.Fakes;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Minimumleeftijd_And_A_Maximumleeftijd
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private const int NieuweMinimumleeftijd = 1;
    private const int NieuweMaximumleeftijd = 142;

    public With_A_Minimumleeftijd_And_A_Maximumleeftijd()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            Doelgroep: Doelgroep.Create(NieuweMinimumleeftijd, NieuweMaximumleeftijd));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_DoelgroepWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new DoelgroepWerdGewijzigd(
                new Registratiedata.Doelgroep(
                    NieuweMinimumleeftijd,
                    NieuweMaximumleeftijd))
        );
    }
}
