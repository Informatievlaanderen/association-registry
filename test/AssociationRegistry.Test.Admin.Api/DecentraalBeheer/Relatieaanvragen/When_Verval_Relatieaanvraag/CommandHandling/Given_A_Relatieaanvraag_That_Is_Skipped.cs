namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Verval_Relatieaanvraag.CommandHandling;

using Xunit;

/// <summary>
///     A command can come twice, or the projection the job read was behind. The handler checks again on the stream: it
///     saves nothing and does not throw, so the message does not end up in the error queue.
/// </summary>
public class Given_A_Relatieaanvraag_That_Is_Skipped
{
    private static readonly DateOnly DayAfterTheVervalDatum = VervalRelatieaanvraagContext.VervalDatum.AddDays(1);

    [Fact]
    public async ValueTask On_The_Vervaldatum_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: VervalRelatieaanvraagContext.VervalDatum);

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask Before_The_Vervaldatum_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: VervalRelatieaanvraagContext.AanvraagDatum.AddDays(1));

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }

    /// <summary>A second command for the same relatieaanvraag.</summary>
    [Fact]
    public async ValueTask When_Already_Vervallen_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: DayAfterTheVervalDatum, alreadyVervallen: true);

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Bevestigd_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: DayAfterTheVervalDatum, alreadyBevestigd: true);

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Geweigerd_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: DayAfterTheVervalDatum, alreadyGeweigerd: true);

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Ingetrokken_Then_Nothing_Is_Saved()
    {
        var ctx = new VervalRelatieaanvraagContext(today: DayAfterTheVervalDatum, alreadyIngetrokken: true);

        await ctx.Verval();

        ctx.ShouldNotHaveSaved();
    }
}
