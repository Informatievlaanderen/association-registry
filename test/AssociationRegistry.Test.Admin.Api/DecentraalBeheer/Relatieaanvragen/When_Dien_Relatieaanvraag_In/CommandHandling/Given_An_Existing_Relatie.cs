namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using Xunit;
using static Verenigingen;

/// <summary>Een koepel en een vereniging kunnen maar één relatie hebben: nooit tegelijk lid en sub.</summary>
public class Given_An_Existing_Relatie
{
    public static TheoryData<string> RelatieTypes =>
        new() { RelatieType.Lidmaatschap.Value, RelatieType.Subvereniging.Value };

    [Theory]
    [MemberData(nameof(RelatieTypes))]
    public async ValueTask With_A_Lidmaatschap_Bij_The_Koepel_Then_It_Throws_ErBestaatAlEenActieveFeitelijkeRelatie(
        string relatieType
    )
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).MetLidmaatschap(KoepelA));

        await Assert.ThrowsAsync<ErBestaatAlEenActieveFeitelijkeRelatie>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Parse(relatieType)))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Lidmaatschap_That_Ends_Today_Then_It_Throws_ErBestaatAlEenActieveFeitelijkeRelatie()
    {
        var lidmaatschapTotVandaag = Vzer(VerenigingC)
            .MetLidmaatschap(KoepelA, van: new DateOnly(2020, 1, 1), tot: DienRelatieaanvraagInContext.Vandaag);

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), lidmaatschapTotVandaag);

        await Assert.ThrowsAsync<ErBestaatAlEenActieveFeitelijkeRelatie>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Theory]
    [MemberData(nameof(RelatieTypes))]
    public async ValueTask With_A_SubverenigingVan_The_Koepel_Then_It_Throws_ErBestaatAlEenActieveFeitelijkeRelatie(
        string relatieType
    )
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).AlsSubverenigingVan(KoepelA));

        await Assert.ThrowsAsync<ErBestaatAlEenActieveFeitelijkeRelatie>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Parse(relatieType)))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Future_Lidmaatschap_Bij_The_Koepel_Then_A_Lidmaatschap_Throws_ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel()
    {
        var toekomstigLidmaatschap = Vzer(VerenigingC)
            .MetLidmaatschap(KoepelA, van: DienRelatieaanvraagInContext.Vandaag.AddMonths(2));

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), toekomstigLidmaatschap);

        await Assert.ThrowsAsync<ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Future_Lidmaatschap_Bij_The_Koepel_Then_A_Subvereniging_Throws_VerenigingMetLidmaatschapKanGeenSubverenigingWorden()
    {
        var toekomstigLidmaatschap = Vzer(VerenigingC)
            .MetLidmaatschap(KoepelA, van: DienRelatieaanvraagInContext.Vandaag.AddMonths(2));

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), toekomstigLidmaatschap);

        await Assert.ThrowsAsync<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }
}
