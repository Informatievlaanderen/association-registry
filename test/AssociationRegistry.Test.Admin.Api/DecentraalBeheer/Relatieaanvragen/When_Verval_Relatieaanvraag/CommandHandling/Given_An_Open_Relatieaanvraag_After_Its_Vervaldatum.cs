namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Verval_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;

/// <summary>From the day after the vervaldatum the job lets the relatieaanvraag vervallen: only the verval is saved.</summary>
public class Given_An_Open_Relatieaanvraag_After_Its_Vervaldatum
{
    private static readonly DateOnly VervalDatum = VervalRelatieaanvraagContext.VervalDatum;

    [Fact]
    public async ValueTask When_The_Job_Runs_The_Day_After_The_Vervaldatum_Then_It_Is_Vervallen_That_Day()
    {
        var today = VervalDatum.AddDays(1);
        var ctx = new VervalRelatieaanvraagContext(today);

        await ctx.Verval();

        ctx.Vervallen.Should()
            .Be(new RelatieaanvraagWerdVervallen(VervalRelatieaanvraagContext.AanvraagId.Value, today));
    }

    /// <summary>The job did not run for a while: the relatieaanvraag vervalt on the day the job runs.</summary>
    [Fact]
    public async ValueTask When_The_Job_Runs_Days_Later_Then_It_Is_Vervallen_On_The_Day_The_Job_Runs()
    {
        var today = VervalDatum.AddDays(10);
        var ctx = new VervalRelatieaanvraagContext(today);

        await ctx.Verval();

        ctx.Vervallen!.VervalDatum.Should().Be(today);
    }

    [Fact]
    public async ValueTask When_It_Is_A_Subverenigingsaanvraag_Then_It_Is_Vervallen()
    {
        var ctx = new VervalRelatieaanvraagContext(today: VervalDatum.AddDays(1), RelatieType.Subvereniging);

        await ctx.Verval();

        ctx.Vervallen.Should().NotBeNull();
    }
}
