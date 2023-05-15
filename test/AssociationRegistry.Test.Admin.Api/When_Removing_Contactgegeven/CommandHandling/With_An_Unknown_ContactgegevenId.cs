namespace AssociationRegistry.Test.Admin.Api.When_Removing_Contactgegeven.CommandHandling;

using Acties.VerwijderContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_ContactgegevenId
{
    private readonly VerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerwijderContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new VerenigingWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();
        _commandHandler = new VerwijderContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownContactgegevenException_Is_Thrown()
    {
        int nietBestaandContactgegevenId;
        var bestaandeContactgegevenIds =
            _scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(x => x.ContactgegevenId)
                .ToArray();
        do
        {
            nietBestaandContactgegevenId = _fixture.Create<int>();
        } while (bestaandeContactgegevenIds.Contains(nietBestaandContactgegevenId));

        var command = new VerwijderContactgegevenCommand(_scenario.VCode, nietBestaandContactgegevenId);
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderContactgegevenCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<OnbekendContactgegeven>();
    }
}
