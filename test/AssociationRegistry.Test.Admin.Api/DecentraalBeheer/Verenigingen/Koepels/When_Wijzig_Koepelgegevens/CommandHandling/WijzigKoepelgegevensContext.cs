namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;

public class WijzigKoepelgegevensContext
{
    private readonly WijzigKoepelgegevensCommandHandler _commandHandler;
    private readonly CommandMetadata _metadata;

    private WijzigKoepelgegevensContext(CommandhandlerScenarioBase scenario)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        _commandHandler = new WijzigKoepelgegevensCommandHandler(AggregateSessionMock);
        _metadata = fixture.Create<CommandMetadata>();
    }

    public CommandhandlerScenarioBase Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }

    public static WijzigKoepelgegevensContext Given(CommandhandlerScenarioBase scenario) => new(scenario);

    public async ValueTask<WijzigKoepelgegevensContext> When(
        Koepelstatus? koepelstatus = null,
        bool? privacyVerklaringGeaccepteerd = null
    )
    {
        var command = new WijzigKoepelgegevensCommand(Scenario.VCode, koepelstatus, privacyVerklaringGeaccepteerd);

        await _commandHandler.Handle(new CommandEnvelope<WijzigKoepelgegevensCommand>(command, _metadata));

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public void ShouldNotHaveSavedAnyEvents() => AggregateSessionMock.ShouldNotHaveAnySaves();
}
