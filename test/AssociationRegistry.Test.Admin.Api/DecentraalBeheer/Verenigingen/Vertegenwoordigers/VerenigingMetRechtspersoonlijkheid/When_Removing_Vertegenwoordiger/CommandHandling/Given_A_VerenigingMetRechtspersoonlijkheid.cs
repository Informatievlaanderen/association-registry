namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingMetRechtspersoonlijkheid.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_A_VerenigingMetRechtspersoonlijkheid
{
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;
    private readonly CommandEnvelope<VerwijderVertegenwoordigerCommand> _envelope;

    public Given_A_VerenigingMetRechtspersoonlijkheid()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = fixture.Create<VerwijderVertegenwoordigerCommand>() with { VCode = scenario.VCode };
        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock, Mock.Of<IMartenOutbox>());
        _envelope = new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata);
    }

    [Fact]
    public async ValueTask Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope);
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();
    }
}
