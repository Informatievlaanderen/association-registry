namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_Null_Values_Does_Not_Update_Anything
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Null_Values_Does_Not_Update_Anything()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_It_Does_Not_Update_Anything()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                Waarde: null,
                Beschrijving: null,
                IsPrimair: null
            )
        );

        await _commandHandler.Handle(
            new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}

public class Given_Null_For_Beschrijving_Does_Not_Update_Beschrijving
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Null_For_Beschrijving_Does_Not_Update_Beschrijving()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_It_Does_Not_Update_Anything()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _fixture.Create<Email>().Waarde,
                Beschrijving: null,
                IsPrimair: true
            )
        );

        await _commandHandler.Handle(
            new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new ContactgegevenWerdGewijzigd(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                Contactgegeventype.Email,
                command.Contactgegeven.Waarde!,
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.Beschrijving, // <== this must stay the same
                IsPrimair: true
            )
        );
    }
}
