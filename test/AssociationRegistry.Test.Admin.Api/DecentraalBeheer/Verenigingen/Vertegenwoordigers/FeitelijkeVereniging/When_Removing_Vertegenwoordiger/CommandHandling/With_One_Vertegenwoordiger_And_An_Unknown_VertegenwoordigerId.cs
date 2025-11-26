namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Moq;
using Wolverine.Marten;
using Xunit;

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
        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock, Mock.Of<IMartenOutbox>());
    }

    [Fact]
    public async ValueTask Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var unKnownVertegenwoordigerId = _scenario.VertegenwoordigerId + 1;
        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, unKnownVertegenwoordigerId);
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<VertegenwoordigerIsNietGekend>();
    }
}
