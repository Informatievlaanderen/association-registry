namespace AssociationRegistry.Test.Admin.Api.When_Removing_Vertegenwoordiger.CommandHandling;

using Acties.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_VertegenwoordigerId
{
    private readonly VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_VertegenwoordigerId()
    {
        _scenario = new VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();
        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        int nietBestaandContactgegevenId;
        var bestaandeContactgegevenIds =
            _scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(x => x.ContactgegevenId)
                .ToArray();
        do
        {
            nietBestaandContactgegevenId = _fixture.Create<int>();
        } while (bestaandeContactgegevenIds.Contains(nietBestaandContactgegevenId));

        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, nietBestaandContactgegevenId);
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<UnknownVertegenwoordiger>();
    }
}
