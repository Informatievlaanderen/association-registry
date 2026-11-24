namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>
///     Only the ontvanger can confirm, while the relatieaanvraag is open and not vervallen, and while the koepel is
///     still a koepel. Nothing is saved when one of these fails.
/// </summary>
public class Given_A_Relatieaanvraag_That_Cannot_Be_Bevestigd
{
    public static TheoryData<string> NotTheOntvanger => new() { KoepelA.Value, "V0001009" };

    [Theory]
    [MemberData(nameof(NotTheOntvanger))]
    public async ValueTask When_Not_The_Ontvanger_Confirms_Then_It_Throws_NamensIsNietDeOntvanger(string namens)
    {
        var ctx = new BevestigRelatieaanvraagContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA
        );

        await Assert.ThrowsAsync<NamensIsNietDeOntvanger>(async () => await ctx.Bevestig(VCode.Create(namens)));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_Already_Bevestigd_Then_It_Throws_RelatieaanvraagWerdAlBevestigd()
    {
        var ctx = new BevestigRelatieaanvraagContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA,
            alreadyBevestigd: true
        );

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlBevestigd>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
        ctx.ShouldNotHaveLoadedTheVerenigingen();
    }

    [Fact]
    public async ValueTask When_Already_Geweigerd_Then_It_Throws_RelatieaanvraagWerdAlGeweigerd()
    {
        var ctx = new BevestigRelatieaanvraagContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA,
            alreadyGeweigerd: true
        );

        await Assert.ThrowsAsync<RelatieaanvraagWerdAlGeweigerd>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
        ctx.ShouldNotHaveLoadedTheVerenigingen();
    }

    [Fact]
    public async ValueTask When_The_Vervaldatum_Is_Over_Then_It_Throws_RelatieaanvraagIsVervallen()
    {
        var vervalDatum = BevestigRelatieaanvraagContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new BevestigRelatieaanvraagContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA,
            today: vervalDatum.AddDays(1)
        );

        await Assert.ThrowsAsync<RelatieaanvraagIsVervallen>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask On_The_Vervaldatum_Then_It_Is_Bevestigd()
    {
        var vervalDatum = BevestigRelatieaanvraagContext.AanvraagDatum.AddDays(Relatieaanvraag.TermijnInDagen);

        var ctx = new BevestigRelatieaanvraagContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA,
            today: vervalDatum
        );

        await ctx.Bevestig(VerenigingC);

        ctx.Bevestigd.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask When_The_Koepel_Is_No_Koepel_Anymore_Then_It_Throws_BeoogdeKoepelIsNietAlsKoepelGeidentificeerd()
    {
        var ctx = new BevestigRelatieaanvraagContext(
            GeenKoepel(KoepelA),
            Vzer(VerenigingC),
            RelatieType.Lidmaatschap,
            aangevraagdDoor: KoepelA
        );

        await Assert.ThrowsAsync<BeoogdeKoepelIsNietAlsKoepelGeidentificeerd>(async () =>
            await ctx.Bevestig(VerenigingC)
        );

        ctx.ShouldNotHaveSaved();
    }
}
