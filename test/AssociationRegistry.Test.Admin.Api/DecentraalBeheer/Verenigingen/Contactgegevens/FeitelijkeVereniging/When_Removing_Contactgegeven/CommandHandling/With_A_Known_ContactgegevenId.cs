namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Removing_Contactgegeven.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class With_A_Known_ContactgegevenId
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public With_A_Known_ContactgegevenId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new VerwijderContactgegevenCommand(_scenario.VCode,
                                                         FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario
                                                            .ContactgegevenId);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderContactgegevenCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(new CommandEnvelope<VerwijderContactgegevenCommand>(command, commandMetadata))
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingOfAnyKind>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_ContactgegevenWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ContactgegevenWerdVerwijderd(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _scenario.Type,
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.Waarde,
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.Beschrijving,
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.IsPrimair)
        );
    }
}
