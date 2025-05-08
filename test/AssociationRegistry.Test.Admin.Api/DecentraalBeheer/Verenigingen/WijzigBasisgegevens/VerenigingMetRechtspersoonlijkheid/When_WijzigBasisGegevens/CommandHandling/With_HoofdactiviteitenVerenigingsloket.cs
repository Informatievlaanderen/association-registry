namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;

public class With_HoofdactiviteitenVerenigingsloket
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly HoofdactiviteitVerenigingsloket[] _hoofdactiviteitenVerenigingsloket;

    public With_HoofdactiviteitenVerenigingsloket()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _hoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray();

        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: _hoofdactiviteitenVerenigingsloket);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_HoofactiviteitenVerenigingloketWerdenGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            EventFactory.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(_hoofdactiviteitenVerenigingsloket)
        );
    }
}
