namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.VerenigingErkendStatusTests;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Primitives;
using Xunit;

public class Given_ErkenningNietLangerActief_And_VerenigingWerdNietLangerErkend
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithVerenigingWerdErkend()
                .WithVerlopenErkenning()
                .WithVerenigingWerdNietLangerErkend()
                .Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask With_Wijzig_Actieve_Erkenning_Then_Saves_SchorsingVanErkenningWerdOpgeheven(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new WijzigErkenningContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startdatum = today.AddDays(-ctx.Fixture.Create<int>());
        var hernieuwingsdatum = today.AddDays(ctx.Fixture.Create<int>());
        var eindDatum = hernieuwingsdatum.AddDays(ctx.Fixture.Create<int>());

        ctx.WithErkenningCommand(cmd =>
            cmd with
            {
                Erkenning = ctx.Command.Erkenning with
                {
                    StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
                    Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
                    EindDatum = NullOrEmpty<DateOnly>.Create(eindDatum),
                    HernieuwingsUrl = ctx.Fixture.Create<HernieuwingsUrl>().Value,
                },
            }
        );

        await ctx.Handle(ctx.Command);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                ctx.Command.Erkenning.ErkenningId,
                ctx.Command.Erkenning.StartDatum.Value,
                ctx.Command.Erkenning.EindDatum.Value,
                ctx.Command.Erkenning.Hernieuwingsdatum.Value,
                ctx.Command.Erkenning.HernieuwingsUrl,
                ErkenningStatus.Actief.Value,
                ctx.Command.Erkenning.RedenVanWijziging
            ),
            new VerenigingWerdErkend()
        );
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask With_Wijzig_Niet_Actieve_Erkenning_Then_Saves_SchorsingVanErkenningWerdOpgeheven(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new WijzigErkenningContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startdatum = today.AddDays(ctx.Fixture.Create<int>());
        var hernieuwingsdatum = startdatum.AddDays(ctx.Fixture.Create<int>());
        var eindDatum = hernieuwingsdatum.AddDays(ctx.Fixture.Create<int>());

        ctx.WithErkenningCommand(cmd =>
            cmd with
            {
                Erkenning = ctx.Command.Erkenning with
                {
                    StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
                    Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
                    EindDatum = NullOrEmpty<DateOnly>.Create(eindDatum),
                    HernieuwingsUrl = ctx.Fixture.Create<HernieuwingsUrl>().Value,
                },
            }
        );

        await ctx.Handle(ctx.Command);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                ctx.Command.Erkenning.ErkenningId,
                ctx.Command.Erkenning.StartDatum.Value,
                ctx.Command.Erkenning.EindDatum.Value,
                ctx.Command.Erkenning.Hernieuwingsdatum.Value,
                ctx.Command.Erkenning.HernieuwingsUrl,
                ErkenningStatus.InAanvraag.Value,
                ctx.Command.Erkenning.RedenVanWijziging
            )
        );
    }
}
