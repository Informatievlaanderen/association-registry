namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Xunit;
using static Verenigingen;

/// <summary>
///     Only the ontvanger can refuse, while the relatieaanvraag is open and not vervallen. Nothing is saved when one of
///     these fails.
/// </summary>
public class Given_A_Relatieaanvraag_That_Cannot_Be_Geweigerd
{
    public static TheoryData<string> NotTheOntvanger => new() { KoepelA.Value, "V0001009" };

    [Theory]
    [MemberData(nameof(NotTheOntvanger))]
    public async ValueTask When_Not_The_Ontvanger_Refuses_Then_It_Throws_NamensIsNietDeOntvanger(string namens)
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA);

        await Assert.ThrowsAsync<NamensIsNietDeOntvanger>(async () => await ctx.Weiger(VCode.Create(namens)));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Geweigerd_Then_It_Throws_RelatieaanvraagWerdAlGeweigerd()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, alreadyGeweigerd: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlGeweigerd>(async () => await ctx.Weiger(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Ingetrokken_Then_It_Throws_RelatieaanvraagWerdAlIngetrokken()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, alreadyIngetrokken: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlIngetrokken>(async () => await ctx.Weiger(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Bevestigd_Then_It_Throws_RelatieaanvraagWerdAlBevestigd()
    {
        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, alreadyBevestigd: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlBevestigd>(async () => await ctx.Weiger(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_The_Vervaldatum_Is_Over_Then_It_Throws_RelatieaanvraagIsVervallen()
    {
        var vervalDatum = WeigerRelatieaanvraagContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new WeigerRelatieaanvraagContext(aangevraagdDoor: KoepelA, today: vervalDatum.AddDays(1));

        await Assert.ThrowsAsync<RelatieaanvraagIsVervallen>(async () => await ctx.Weiger(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }
}
