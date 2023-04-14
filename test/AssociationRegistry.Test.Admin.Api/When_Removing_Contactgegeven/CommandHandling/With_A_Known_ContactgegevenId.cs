namespace AssociationRegistry.Test.Admin.Api.When_Removing_Contactgegeven.CommandHandling;

using Acties.VerwijderContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Known_ContactgegevenId
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario _scenario;

    public With_A_Known_ContactgegevenId()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        var fixture = new Fixture().CustomizeAll();
        var command = new VerwijderContactgegevenCommand(_scenario.VCode, _scenario.ContactgegevenId);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderContactgegevenCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(new CommandEnvelope<VerwijderContactgegevenCommand>(command, commandMetadata))
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_ContactgegevenWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdVerwijderd(
                _scenario.ContactgegevenId,
                _scenario.Type,
                _scenario.Waarde,
                _scenario.Beschrijving,
                _scenario.IsPrimair)
        );
    }
}
