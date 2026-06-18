namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Hernieuwingsdatum_Outside_Erkenningsperiode
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_Hernieuwingsdatum_Before_Erkenning_Startdatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        var hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value.AddDays(-1));

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = hernieuwingsdatum,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }

    [Fact]
    public async ValueTask With_Hernieuwingsdatum_After_Erkenning_Einddatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        var hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = hernieuwingsdatum,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }
}
