namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AutoFixture;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_KorteBeschrijving
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private const string NieuweKorteBeschrijving = "Een nieuwe beschrijving van de vereniging";

    public With_A_KorteBeschrijving()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, KorteBeschrijving: NieuweKorteBeschrijving);
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
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_KorteBeschrijvingWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new KorteBeschrijvingWerdGewijzigd(_scenario.VCode, NieuweKorteBeschrijving)
        );
    }
}
