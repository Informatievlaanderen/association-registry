namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging.Emails;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_ContactgegevenId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens _scenario;
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAll();
        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownContactgegevenException_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(_scenario.VCode, new WijzigContactgegevenCommand.CommandContactgegeven(
            _fixture.Create<int>(),
            _fixture.Create<Email>().Waarde,
            _fixture.Create<string?>(),
            IsPrimair: false));
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<OnbekendContactgegeven>();
    }
}
