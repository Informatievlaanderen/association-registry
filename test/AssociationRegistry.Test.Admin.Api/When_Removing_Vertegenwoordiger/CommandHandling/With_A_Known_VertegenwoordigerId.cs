namespace AssociationRegistry.Test.Admin.Api.When_Removing_Vertegenwoordiger.CommandHandling;

using Acties.VerwijderVertegenwoordiger;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using AutoFixture;
using Fixtures.Scenarios.CommandHandling;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Known_VertegenwoordigerId
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;

    public With_A_Known_VertegenwoordigerId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        var fixture = new Fixture().CustomizeAll();
        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderVertegenwoordigerCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata))
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VertegenwoordigerWerdVerwijderd(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                _scenario.VertegenwoordigerWerdToegevoegd.Achternaam)
        );
    }
}
