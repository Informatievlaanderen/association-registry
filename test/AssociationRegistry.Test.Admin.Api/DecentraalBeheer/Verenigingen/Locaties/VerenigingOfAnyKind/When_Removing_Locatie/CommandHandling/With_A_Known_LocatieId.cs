namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.VerwijderLocatie;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Vereniging.Geotags;
using Xunit;

public class With_A_Known_LocatieId
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithMultipleLocatiesScenario _scenario;

    public With_A_Known_LocatieId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithMultipleLocatiesScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new VerwijderLocatieCommand(_scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie.LocatieId);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderLocatieCommandHandler(_verenigingRepositoryMock, Mock.Of<IGeotagsService>());

        commandHandler.Handle(new CommandEnvelope<VerwijderLocatieCommand>(command, commandMetadata))
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingOfAnyKind>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdVerwijderd(
                _scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie),
            new GeotagsWerdenBepaald(_scenario.VCode, [])
        );
    }
}
