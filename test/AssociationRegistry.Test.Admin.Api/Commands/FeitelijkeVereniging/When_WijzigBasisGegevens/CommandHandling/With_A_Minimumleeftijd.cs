namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Acties.WijzigBasisgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Framework.Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Minimumleeftijd
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private const int NieuweMinimumleeftijd = 1;

    public With_A_Minimumleeftijd()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            Doelgroep: Doelgroep.Create(NieuweMinimumleeftijd, maximumleeftijd: null));

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
                    Doelgroep.StandaardMaximumleeftijd))
        );
    }
}
