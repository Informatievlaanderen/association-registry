namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.CommandHandling;

using Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using AutoFixture;
using Fixtures.Scenarios.CommandHandling;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null_Values_Does_Not_Update_Anything
{
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_Null_Values_Does_Not_Update_Anything()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigVertegenwoordigerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_It_Does_Not_Update_Anything()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                null,
                null,
                null,
                null,
                null,
                null,
                null));

        await _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
