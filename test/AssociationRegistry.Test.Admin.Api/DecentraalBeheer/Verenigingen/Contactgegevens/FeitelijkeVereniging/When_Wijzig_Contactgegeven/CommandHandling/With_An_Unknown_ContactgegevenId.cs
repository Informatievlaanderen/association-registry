namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
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
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithoutContactgegevens();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_UnknownContactgegevenException_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(_scenario.VCode, new WijzigContactgegevenCommand.CommandContactgegeven(
                                                          _fixture.Create<int>(),
                                                          _fixture.Create<Email>().Waarde,
                                                          _fixture.Create<string?>(),
                                                          IsPrimair: false));

        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<ContactgegevenIsNietGekend>();
    }
}
