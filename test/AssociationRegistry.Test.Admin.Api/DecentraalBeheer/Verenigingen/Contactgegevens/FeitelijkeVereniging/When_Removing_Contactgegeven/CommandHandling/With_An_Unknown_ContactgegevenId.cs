namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Removing_Contactgegeven.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class With_An_Unknown_ContactgegevenId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens _scenario;
    private readonly VerwijderContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens();

        var verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_UnknownContactgegevenException_Is_Thrown()
    {
        var command = new VerwijderContactgegevenCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () =>
            _commandHandler.Handle(new CommandEnvelope<VerwijderContactgegevenCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<ContactgegevenIsNietGekend>();
    }
}
