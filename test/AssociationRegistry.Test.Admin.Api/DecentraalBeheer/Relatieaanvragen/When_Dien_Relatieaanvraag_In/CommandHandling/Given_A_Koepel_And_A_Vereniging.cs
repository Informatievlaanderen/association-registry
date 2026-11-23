namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

public class Given_A_Koepel_And_A_Vereniging
{
    [Fact]
    public async ValueTask When_The_Koepel_Asks_A_Lidmaatschap_Then_It_Saves_RelatieaanvraagWerdIngediend()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC));

        var result = await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
        result.AanvraagId.Should().Be(DienRelatieaanvraagInContext.AanvraagId);
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Asks_To_Become_Subvereniging_Then_The_Vereniging_Is_The_Aanvrager()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC));

        await ctx.Handle(ctx.Command(RelatieType.Subvereniging, aangevraagdDoor: VerenigingC));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Subvereniging, aangevraagdDoor: VerenigingC));
    }

    [Fact]
    public async ValueTask When_A_Vereniging_With_Subtype_NietBepaald_Asks_To_Become_Subvereniging_Then_It_Is_Saved()
    {
        var nietBepaald = Vzer(VerenigingC)
            .Apply(new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(VerenigingC.Value));
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), nietBepaald);

        await ctx.Handle(ctx.Command(RelatieType.Subvereniging, aangevraagdDoor: VerenigingC));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Subvereniging, aangevraagdDoor: VerenigingC));
    }

    [Fact]
    public async ValueTask When_A_Vereniging_Met_Rechtspersoonlijkheid_Asks_A_Lidmaatschap_Then_It_Is_Saved()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), KboVereniging(VerenigingC));

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
    }

    [Fact]
    public async ValueTask When_The_Privacyverklaring_Is_Not_Accepted_Then_It_Is_Saved()
    {
        var koepelZonderPrivacyverklaring = Koepel(KoepelA);
        koepelZonderPrivacyverklaring.Koepelgegevens.PrivacyVerklaringGeaccepteerd.Should().BeFalse();

        var ctx = new DienRelatieaanvraagInContext(koepelZonderPrivacyverklaring, Vzer(VerenigingC));

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
    }

    [Fact]
    public async ValueTask When_Two_Koepels_Become_Lid_Of_Each_Other_Then_It_Is_Saved()
    {
        // Koepel A is lid van koepel B; koepel B vraagt om lid van koepel A te worden: ketens tussen koepels mogen.
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA).MetLidmaatschap(KoepelB), Koepel(KoepelB));

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelB));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelB));
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Was_Lid_Of_The_Koepel_Before_Then_It_Is_Saved()
    {
        var beeindigdLidmaatschap = Vzer(VerenigingC)
            .MetLidmaatschap(
                KoepelA,
                van: new DateOnly(2020, 1, 1),
                tot: DienRelatieaanvraagInContext.Vandaag.AddDays(-1)
            );

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), beeindigdLidmaatschap);

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Is_Lid_Of_Another_Koepel_Then_A_Lidmaatschap_Is_Saved()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).MetLidmaatschap(KoepelB));

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
    }
}
