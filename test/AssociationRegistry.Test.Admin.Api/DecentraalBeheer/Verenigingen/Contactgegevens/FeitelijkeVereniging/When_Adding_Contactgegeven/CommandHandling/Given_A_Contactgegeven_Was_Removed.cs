namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Xunit;

public class Given_A_Contactgegeven_Was_Removed
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithRemovedContactgegevenScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Contactgegeven_Was_Removed()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithRemovedContactgegevenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved_With_The_Next_Id3()
    {
        var command = _fixture.Create<VoegContactgegevenToeCommand>() with { VCode = _scenario.VCode };

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(ContactgegevenId: 2, command.Contactgegeven.Contactgegeventype, command.Contactgegeven.Waarde,
                                             command.Contactgegeven.Beschrijving, IsPrimair: false)
        );
    }
}
