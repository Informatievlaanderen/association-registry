namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Removing_Contactgegeven.CommandHandling;

using Acties.VerwijderContactgegeven;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_ContactgegevenId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerwijderContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

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
