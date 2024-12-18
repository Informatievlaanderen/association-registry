namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using Acties.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_One_Vertegenwoordiger_And_An_Unknown_VertegenwoordigerId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario _scenario;
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_One_Vertegenwoordiger_And_An_Unknown_VertegenwoordigerId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<VertegenwoordigerIsNietGekend>();
    }
}
