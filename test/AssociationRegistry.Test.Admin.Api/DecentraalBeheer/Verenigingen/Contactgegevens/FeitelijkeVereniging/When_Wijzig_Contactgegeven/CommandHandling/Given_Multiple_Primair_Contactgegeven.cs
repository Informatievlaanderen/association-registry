namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_Multiple_Primair_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario _scenario;

    public Given_Multiple_Primair_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_MultiplePrimaryContactgegevens_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                _scenario.ContactgegevenWerdToegevoegd2.ContactgegevenId,
                _scenario.ContactgegevenWerdToegevoegd2.Waarde,
                _scenario.ContactgegevenWerdToegevoegd2.Beschrijving,
                IsPrimair: true)); // <== changed value

        var handle = ()
            => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        await handle.Should().ThrowAsync<MeerderePrimaireContactgegevensZijnNietToegestaan>();
    }
}
