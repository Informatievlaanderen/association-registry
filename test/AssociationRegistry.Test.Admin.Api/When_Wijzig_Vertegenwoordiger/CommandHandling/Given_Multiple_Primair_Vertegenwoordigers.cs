namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.CommandHandling;

using Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging.Exceptions;
using AutoFixture;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Multiple_Primair_Vertegenwoordigers
{
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;

    public Given_Multiple_Primair_Vertegenwoordigers()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_MultiplePrimaryVertegenwoordiger_Is_Thrown()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _scenario.VertegenwoordigerWerdToegevoegd2.VertegenwoordigerId,
                null,
                null,
                null,
                null,
                null,
                null,
                IsPrimair: true)); // <== changed value

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, _fixture.Create<CommandMetadata>()));
        await handle.Should().ThrowAsync<MultiplePrimaryVertegenwoordigers>();
    }
}
