namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Framework.Fakes;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_NietBepaald_Werkingsgebieden
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly NietBepaaldWerkingsgebiedenScenario _scenario;
    private readonly Werkingsgebied[] _werkingsgebieden;

    public With_NietBepaald_Werkingsgebieden()
    {
        _scenario = new NietBepaaldWerkingsgebiedenScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _werkingsgebieden = fixture.CreateMany<Werkingsgebied>().Distinct().ToArray();

        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Werkingsgebieden: _werkingsgebieden);

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

    // TODO: fix state of registreer fv
    [Fact]
    public void Then_A_WerkingsgebiedenWerdenGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            WerkingsgebiedenWerdenBepaald.With(_werkingsgebieden)
        );
    }
}
