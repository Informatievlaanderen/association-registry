namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Kbo;

using System;
using System.Threading.Tasks;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Erkenning_Cannot_Be_Activated
{
    private readonly Fixture _fixture = new Fixture().CustomizeAdminApi();

    [Fact]
    public async ValueTask Given_Startdatum_In_Future_Then_Does_Not_Save_An_ErkenningWerdGeactiveerd_Event()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        await AssertNoErkenningWerdGeactiveerdEvent(
            startdatum: today.AddDays(1),
            einddatum: today.AddYears(1),
            status: ErkenningStatus.InAanvraag.Value
        );
    }

    [Fact]
    public async ValueTask Given_Active_Erkenning_Then_Does_Not_Save_An_ErkenningWerdGeactiveerd_Event()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        await AssertNoErkenningWerdGeactiveerdEvent(
            startdatum: today.AddDays(-1),
            einddatum: today.AddYears(1),
            status: ErkenningStatus.Actief.Value
        );
    }

    [Fact]
    public async ValueTask Given_Geschorste_Erkenning_Then_Does_Not_Save_An_ErkenningWerdGeactiveerd_Event()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        await AssertNoErkenningWerdGeactiveerdEvent(
            startdatum: today.AddDays(-1),
            einddatum: today.AddYears(1),
            status: ErkenningStatus.Geschorst.Value
        );
    }

    [Fact]
    public async ValueTask Given_Verlopen_Erkenning_Then_Does_Not_Save_An_ErkenningWerdGeactiveerd_Event()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        await AssertNoErkenningWerdGeactiveerdEvent(
            startdatum: today.AddYears(-2),
            einddatum: today.AddDays(-1),
            status: ErkenningStatus.Verlopen.Value
        );
    }

    private async ValueTask AssertNoErkenningWerdGeactiveerdEvent(
        DateOnly startdatum,
        DateOnly einddatum,
        string status
    )
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario(
            startdatum,
            einddatum,
            status
        );

        var aggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        var commandHandler = new ActiveerErkenningCommandHandler(aggregateSessionMock);

        var command = _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = scenario.VCode,
            ErkenningId = scenario.ErkenningWerdGeregistreerd.ErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () =>
        {
            await commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata));
        });

        aggregateSessionMock.ShouldNotHaveAnySaves();

        exception
            .Message.Should()
            .Be(
                string.Format(
                    "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
                    scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    startdatum,
                    einddatum,
                    scenario.ErkenningWerdGeregistreerd.Status
                )
            );
    }
}
