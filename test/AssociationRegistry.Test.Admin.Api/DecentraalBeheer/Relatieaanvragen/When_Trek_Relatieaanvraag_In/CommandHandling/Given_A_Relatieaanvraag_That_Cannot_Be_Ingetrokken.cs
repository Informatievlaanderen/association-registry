namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Xunit;
using static Verenigingen;

/// <summary>
///     Only the aanvrager can withdraw, while the relatieaanvraag is open and not vervallen. Nothing is saved when one of
///     these fails.
/// </summary>
public class Given_A_Relatieaanvraag_That_Cannot_Be_Ingetrokken
{
    public static TheoryData<string> NotTheAanvrager => new() { VerenigingC.Value, "V0001009" };

    [Theory]
    [MemberData(nameof(NotTheAanvrager))]
    public async ValueTask When_Not_The_Aanvrager_Withdraws_Then_It_Throws_NamensIsNietDeAanvrager(string namens)
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA);

        await Assert.ThrowsAsync<NamensIsNietDeAanvrager>(async () => await ctx.TrekIn(VCode.Create(namens)));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Ingetrokken_Then_It_Throws_RelatieaanvraagWerdAlIngetrokken()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA, alreadyIngetrokken: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlIngetrokken>(async () => await ctx.TrekIn(KoepelA));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Bevestigd_Then_It_Throws_RelatieaanvraagWerdAlBevestigd()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA, alreadyBevestigd: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlBevestigd>(async () => await ctx.TrekIn(KoepelA));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Geweigerd_Then_It_Throws_RelatieaanvraagWerdAlGeweigerd()
    {
        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA, alreadyGeweigerd: true);

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlGeweigerd>(async () => await ctx.TrekIn(KoepelA));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_The_Vervaldatum_Is_Over_Then_It_Throws_RelatieaanvraagIsVervallen()
    {
        var vervalDatum = TrekRelatieaanvraagInContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new TrekRelatieaanvraagInContext(aangevraagdDoor: KoepelA, today: vervalDatum.AddDays(1));

        await Assert.ThrowsAsync<RelatieaanvraagIsVervallen>(async () => await ctx.TrekIn(KoepelA));

        ctx.ShouldNotHaveSaved();
    }
}
