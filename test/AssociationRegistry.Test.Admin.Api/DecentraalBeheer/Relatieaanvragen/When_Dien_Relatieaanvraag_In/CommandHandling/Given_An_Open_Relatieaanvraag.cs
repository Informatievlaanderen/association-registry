namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using FluentAssertions;
using Xunit;
using static Verenigingen;

public class Given_An_Open_Relatieaanvraag
{
    private static readonly RelatieaanvraagId OpenAanvraagId = RelatieaanvraagId.Create(7);

    private static readonly OpenRelatieaanvraag OpenLidmaatschapsaanvraag = new(
        OpenAanvraagId,
        RelatieType.Lidmaatschap
    );
    private static readonly OpenRelatieaanvraag OpenSubverenigingsaanvraag = new(
        OpenAanvraagId,
        RelatieType.Subvereniging
    );

    [Fact]
    public async ValueTask Of_The_Same_Type_Then_It_Throws_RelatieaanvraagVanHetzelfdeTypeStaatAlOpen()
    {
        // ongeacht de aanvrager: de open aanvraag kan ook door de vereniging ingediend zijn
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                OpenLidmaatschapsaanvraag,
                Subverenigingsaanvraag: null,
                Lidmaatschapsaanvragen: [OpenLidmaatschapsaanvraag]
            )
        );

        var exception = await Assert.ThrowsAsync<RelatieaanvraagVanHetzelfdeTypeStaatAlOpen>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        exception.Message.Should().Contain(OpenAanvraagId.Value);
        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask Of_Another_Type_Then_It_Throws_RelatieaanvraagVanEenAnderTypeStaatAlOpen()
    {
        // de open subverenigingsaanvraag voor dit paar is ook de open subverenigingsaanvraag van de vereniging
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(OpenSubverenigingsaanvraag, OpenSubverenigingsaanvraag, Lidmaatschapsaanvragen: [])
        );

        await Assert.ThrowsAsync<RelatieaanvraagVanEenAnderTypeStaatAlOpen>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask To_Become_Subvereniging_Of_Another_Koepel_Then_A_Subvereniging_Throws_SubverenigingsaanvraagStaatAlOpen()
    {
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                VoorKoepelEnVereniging: null,
                OpenSubverenigingsaanvraag,
                Lidmaatschapsaanvragen: []
            )
        );

        var exception = await Assert.ThrowsAsync<SubverenigingsaanvraagStaatAlOpen>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        exception.Message.Should().Contain(OpenAanvraagId.Value);
        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask To_Become_Subvereniging_Of_Another_Koepel_Then_A_Lidmaatschap_Throws_VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden()
    {
        // K6: geen subvereniging en lid tegelijk, ook niet via een open aanvraag
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                VoorKoepelEnVereniging: null,
                OpenSubverenigingsaanvraag,
                Lidmaatschapsaanvragen: []
            )
        );

        var exception = await Assert.ThrowsAsync<VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        exception.Message.Should().EndWith($"(relatieaanvraag: '{OpenAanvraagId.Value}').");
        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask To_Become_Lid_Of_Another_Koepel_Then_A_Subvereniging_Throws_VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden()
    {
        // K6: geen subvereniging en lid tegelijk, ook niet via een open aanvraag
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                VoorKoepelEnVereniging: null,
                Subverenigingsaanvraag: null,
                Lidmaatschapsaanvragen: [OpenLidmaatschapsaanvraag]
            )
        );

        var exception = await Assert.ThrowsAsync<VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden>(
            async () =>
                await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        exception.Message.Should().EndWith($"(relatieaanvraag: '{OpenAanvraagId.Value}').");
        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask To_Become_Lid_Of_Other_Koepels_Then_The_Message_Names_Every_Relatieaanvraag()
    {
        var andereOpenLidmaatschapsaanvraag = new OpenRelatieaanvraag(
            RelatieaanvraagId.Create(8),
            RelatieType.Lidmaatschap
        );

        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                VoorKoepelEnVereniging: null,
                Subverenigingsaanvraag: null,
                Lidmaatschapsaanvragen: [OpenLidmaatschapsaanvraag, andereOpenLidmaatschapsaanvraag]
            )
        );

        var exception = await Assert.ThrowsAsync<VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden>(
            async () =>
                await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        exception
            .Message.Should()
            .EndWith(
                $"(relatieaanvragen: '{OpenAanvraagId.Value}', '{andereOpenLidmaatschapsaanvraag.AanvraagId.Value}')."
            );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask To_Become_Lid_Of_Another_Koepel_Then_A_Lidmaatschap_Is_Saved()
    {
        // meerdere open lidmaatschapsaanvragen naar verschillende koepels mogen (0.4)
        var ctx = new DienRelatieaanvraagInContext(
            Koepel(KoepelA),
            Vzer(VerenigingC),
            new OpenRelatieaanvragen(
                VoorKoepelEnVereniging: null,
                Subverenigingsaanvraag: null,
                Lidmaatschapsaanvragen: [OpenLidmaatschapsaanvraag]
            )
        );

        await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap));

        ctx.ShouldHaveSaved(ctx.Ingediend(RelatieType.Lidmaatschap, aangevraagdDoor: KoepelA));
    }
}
