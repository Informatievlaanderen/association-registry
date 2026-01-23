namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_Contactgegeven_With_Same_Type_And_Value_But_Diff_Beschrijving
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Contactgegeven_With_Same_Type_And_Value_But_Diff_Beschrijving()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_DuplicateContactgegeven_Is_Not_Thrown()
    {
        var command = _fixture.Create<VoegContactgegevenToeCommand>() with { VCode = _scenario.VCode };

        await _commandHandler.Handle(
            new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>())
        );

        var secondCommand = command with
        {
            Contactgegeven = command.Contactgegeven with { Beschrijving = _fixture.Create<string>() },
        };

        var secondCall = () =>
            _commandHandler.Handle(
                new CommandEnvelope<VoegContactgegevenToeCommand>(secondCommand, _fixture.Create<CommandMetadata>())
            );

        await secondCall.Should().NotThrowAsync<ContactgegevenIsDuplicaat>();
    }
}
