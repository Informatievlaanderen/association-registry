namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Startdatum_After_Erkenning_Einddatum
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_TeCorrigeren_Einddatum_Null_Then_It_Throws_StartdatumLigtNaEinddatum()
    {
        var startDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = startDatum,
            EindDatum = NullOrEmpty<DateOnly>.Null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }

    [Fact]
    public async ValueTask With_TeCorrigeren_Einddatum_Then_It_Throws_StartdatumLigtNaEinddatum()
    {
        var startDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));
        var eindDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(-1));

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = startDatum,
            EindDatum = eindDatum,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}
