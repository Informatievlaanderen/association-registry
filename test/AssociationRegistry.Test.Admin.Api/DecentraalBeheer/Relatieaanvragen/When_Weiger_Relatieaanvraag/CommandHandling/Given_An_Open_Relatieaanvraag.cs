namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>The ontvanger refuses: only the refusal is saved.</summary>
public class Given_An_Open_Relatieaanvraag
{
    private static readonly RelatieaanvraagId AanvraagId = WeigerRelatieaanvraagContext.AanvraagId;

    [Fact]
    public async ValueTask When_The_Vereniging_Refuses_Then_It_Is_Geweigerd()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA);

        var result = await ctx.Weiger(VerenigingC);

        ctx.Geweigerd.Should()
            .Be(new RelatieaanvraagWerdGeweigerd(AanvraagId.Value, WeigerRelatieaanvraagContext.Today));

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
    public async ValueTask On_The_Vervaldatum_Then_It_Is_Geweigerd()
    {
        var vervalDatum = WeigerRelatieaanvraagContext.AanvraagDatum.AddDays(90);

        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, today: vervalDatum);

        await ctx.Weiger(VerenigingC);

        ctx.Geweigerd!.WeigeringsDatum.Should().Be(vervalDatum);
    }
}
