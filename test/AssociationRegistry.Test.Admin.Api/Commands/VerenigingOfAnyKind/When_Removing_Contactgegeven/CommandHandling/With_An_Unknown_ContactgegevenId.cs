namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Contactgegeven.CommandHandling;

using Acties.VerwijderContactgegeven;
using AssociationRegistry.Framework;
using Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_ContactgegevenId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens _scenario;
    private readonly VerwijderContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownContactgegevenException_Is_Thrown()
    {
        var command = new VerwijderContactgegevenCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderContactgegevenCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<ContactgegevenIsNietGekend>();
    }
}
