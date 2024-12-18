namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
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
public class With_The_Same_KorteNaam
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public With_The_Same_KorteNaam()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, KorteNaam: _scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam);
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
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveSaved<KorteNaamWerdGewijzigd>();
    }
}
