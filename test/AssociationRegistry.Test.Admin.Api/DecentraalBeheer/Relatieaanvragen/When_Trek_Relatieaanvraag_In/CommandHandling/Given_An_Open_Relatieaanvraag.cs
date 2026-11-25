namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>The aanvrager withdraws: only the intrekking is saved, with the reden.</summary>
public class Given_An_Open_Relatieaanvraag
{
    private static readonly RelatieaanvraagId AanvraagId = TrekRelatieaanvraagInContext.AanvraagId;

    [Fact]
    public async ValueTask When_The_Koepel_Withdraws_Its_Relatieaanvraag_Then_It_Is_Ingetrokken_With_The_Reden()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA);

        var result = await ctx.TrekIn(KoepelA, "De aanvraag werd per vergissing ingediend.");

        ctx.Ingetrokken.Should()
            .Be(
                new RelatieaanvraagWerdIngetrokken(
                    AanvraagId.Value,
                    TrekRelatieaanvraagInContext.Today,
                    "De aanvraag werd per vergissing ingediend."
                )
            );

        result.AanvraagId.Should().Be(AanvraagId);
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Withdraws_Its_Subverenigingsaanvraag_Then_It_Is_Ingetrokken()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: VerenigingC, RelatieType.Subvereniging);

        await ctx.TrekIn(VerenigingC);

        ctx.Ingetrokken.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask Without_A_Reden_Then_The_Reden_Is_Empty()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA);

        await ctx.TrekIn(KoepelA);

        ctx.Ingetrokken!.Reden.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask On_The_Vervaldatum_Then_It_Is_Ingetrokken()
    {
        var vervalDatum = TrekRelatieaanvraagInContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA, today: vervalDatum);

        await ctx.TrekIn(KoepelA);

        ctx.Ingetrokken!.IntrekkingsDatum.Should().Be(vervalDatum);
    }
}
