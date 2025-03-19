namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.VerwijderLocatie;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_LocatieId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario _scenario;
    private readonly VerwijderLocatieCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_LocatieId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderLocatieCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var nonExistingLocatieId = _scenario.LocatieWerdToegevoegd.Locatie.LocatieId + _fixture.Create<int>();
        var command = new VerwijderLocatieCommand(_scenario.VCode, nonExistingLocatieId);
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderLocatieCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<LocatieIsNietGekend>();
    }
}
