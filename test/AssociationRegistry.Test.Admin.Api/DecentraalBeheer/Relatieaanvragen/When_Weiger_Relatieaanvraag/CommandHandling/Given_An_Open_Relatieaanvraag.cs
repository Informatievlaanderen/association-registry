namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>The ontvanger refuses: only the refusal is saved, with the reden.</summary>
public class Given_An_Open_Relatieaanvraag
{
    private static readonly RelatieaanvraagId AanvraagId = WeigerRelatieaanvraagContext.AanvraagId;

    [Fact]
    public async ValueTask When_The_Vereniging_Refuses_Then_It_Is_Geweigerd_With_The_Reden()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA);

        var result = await ctx.Weiger(VerenigingC, "Wij zijn geen lid van deze koepel.");

        ctx.Geweigerd.Should()
            .Be(
                new RelatieaanvraagWerdGeweigerd(
                    AanvraagId.Value,
                    WeigerRelatieaanvraagContext.Today,
                    "Wij zijn geen lid van deze koepel."
                )
            );

        result.AanvraagId.Should().Be(AanvraagId);
    }

    [Fact]
    public async ValueTask When_The_Koepel_Refuses_A_Subverenigingsaanvraag_Of_The_Vereniging_Then_It_Is_Geweigerd()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: VerenigingC, RelatieType.Subvereniging);

        await ctx.Weiger(KoepelA);

        ctx.Geweigerd.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask Without_A_Reden_Then_The_Reden_Is_Empty()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA);

        await ctx.Weiger(VerenigingC);

        ctx.Geweigerd!.Reden.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask On_The_Vervaldatum_Then_It_Is_Geweigerd()
    {
        var vervalDatum = WeigerRelatieaanvraagContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, today: vervalDatum);

        await ctx.Weiger(VerenigingC);

        ctx.Geweigerd!.WeigeringsDatum.Should().Be(vervalDatum);
    }
}
