namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using DecentraalBeheer.Locaties.VerwijderLocatie;
using FluentAssertions;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_LocatieId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario _scenario;
    private readonly VerwijderLocatieCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_LocatieId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderLocatieCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = new VerwijderLocatieCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderLocatieCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<LocatieIsNietGekend>();
    }
}
