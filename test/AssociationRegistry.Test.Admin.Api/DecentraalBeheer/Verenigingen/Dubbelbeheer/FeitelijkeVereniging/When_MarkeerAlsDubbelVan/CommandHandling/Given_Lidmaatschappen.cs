namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
///     De authentieke vereniging is lid bij
///     <list type="bullet">
///         <item>Rohan van 2020 tot 2021 (id 1),</item>
///         <item>Gondor vanaf 2024 (id 2),</item>
///         <item>Erebor in 2018-2019 (id 3) en in 2022-2023 (id 4),</item>
///         <item>Lothlórien van 2010 tot 2030 (id 5).</item>
///     </list>
///     De dubbele vereniging is lid bij
///     <list type="bullet">
///         <item>Rohan van midden 2019 tot midden 2020: overlapt, de periode wordt 2019-2021,</item>
///         <item>Gondor zonder startdatum tot 2025: overlapt, de periode wordt open aan beide kanten,</item>
///         <item>Erebor van 2019 tot 2022: overlapt met twee lidmaatschappen, niet overgenomen,</item>
///         <item>Lothlórien in 2015-2016: valt binnen de bestaande periode, gededupliceerd,</item>
///         <item>Rohan vanaf 2025: overlapt niet, toegevoegd als nieuw lidmaatschap met id 6,</item>
///         <item>Isengard (verwijderd), Moria (dubbel), Dale (gestopt) en de authentieke vereniging zelf: niet overgenomen.</item>
///     </list>
/// </summary>
#pragma warning disable CS0618
public class Given_Lidmaatschappen
{
    private static readonly Fixture Fixture = new Fixture().CustomizeDomain();

    private static readonly VCode Rohan = Fixture.Create<VCode>();
    private static readonly VCode Gondor = Fixture.Create<VCode>();
    private static readonly VCode Erebor = Fixture.Create<VCode>();
    private static readonly VCode Lothlorien = Fixture.Create<VCode>();
    private static readonly VCode Isengard = Fixture.Create<VCode>();
    private static readonly VCode Moria = Fixture.Create<VCode>();
    private static readonly VCode Dale = Fixture.Create<VCode>();

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd _authentieke = Fixture
        .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
        .WithBasisgegevens(TestBasisgegevens.Empty);

    private readonly FeitelijkeVerenigingWerdGeregistreerd _dubbele = Fixture
        .Create<FeitelijkeVerenigingWerdGeregistreerd>()
        .WithBasisgegevens(TestBasisgegevens.Empty)
        .WithHoofdactiviteiten([])
        .WithContactgegevens([])
        .WithLocaties([])
        .WithVertegenwoordigers([]);

    private static readonly Registratiedata.Lidmaatschap AuthentiekBijRohan = Lid(
        1,
        Rohan,
        "Rohan",
        new DateOnly(2020, 1, 1),
        new DateOnly(2021, 12, 31)
    ) with
    {
        Identificatie = "ROHAN-AUTHENTIEK",
        Beschrijving = "Ruiters van Rohan",
    };

    private static readonly Registratiedata.Lidmaatschap AuthentiekBijGondor = Lid(
        2,
        Gondor,
        "Gondor",
        new DateOnly(2024, 1, 1),
        null
    );

    private static readonly Registratiedata.Lidmaatschap AuthentiekBijEreborEerst = Lid(
        3,
        Erebor,
        "Erebor",
        new DateOnly(2018, 1, 1),
        new DateOnly(2019, 12, 31)
    );

    private static readonly Registratiedata.Lidmaatschap AuthentiekBijEreborDaarna = Lid(
        4,
        Erebor,
        "Erebor",
        new DateOnly(2022, 1, 1),
        new DateOnly(2023, 12, 31)
    );

    private static readonly Registratiedata.Lidmaatschap AuthentiekBijLothlorien = Lid(
        5,
        Lothlorien,
        "Lothlórien",
        new DateOnly(2010, 1, 1),
        new DateOnly(2030, 12, 31)
    );

    private static readonly Registratiedata.Lidmaatschap OverlappendBijRohan = Lid(
        1,
        Rohan,
        "Rohan",
        new DateOnly(2019, 6, 1),
        new DateOnly(2020, 6, 30)
    ) with
    {
        Identificatie = "ROHAN-DUBBEL",
        Beschrijving = "Andere beschrijving",
    };

    private static readonly Registratiedata.Lidmaatschap OverlappendBijGondor = Lid(
        2,
        Gondor,
        "Gondor",
        null,
        new DateOnly(2025, 12, 31)
    );

    private static readonly Registratiedata.Lidmaatschap OverlappendMetTweeBijErebor = Lid(
        3,
        Erebor,
        "Erebor",
        new DateOnly(2019, 1, 1),
        new DateOnly(2022, 6, 30)
    );

    private static readonly Registratiedata.Lidmaatschap BinnenDePeriodeBijLothlorien = Lid(
        4,
        Lothlorien,
        "Lothlórien",
        new DateOnly(2015, 1, 1),
        new DateOnly(2016, 12, 31)
    );

    private static readonly Registratiedata.Lidmaatschap LaterBijRohan = Lid(
        5,
        Rohan,
        "Rohan",
        new DateOnly(2025, 1, 1),
        null
    );

    private static readonly Registratiedata.Lidmaatschap BijIsengard = Lid(6, Isengard, "Isengard", null, null);
    private static readonly Registratiedata.Lidmaatschap BijMoria = Lid(7, Moria, "Moria", null, null);
    private static readonly Registratiedata.Lidmaatschap BijDale = Lid(8, Dale, "Dale", null, null);

    private Registratiedata.Lidmaatschap BijDeAuthentiekeVereniging =>
        Lid(9, VCode.Create(_authentieke.VCode), "Authentiek", null, null);

    private async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan()
    {
        var session = new MultipleVerenigingAggregateSessionMock(
            new Registratiedata.Lidmaatschap[]
            {
                OverlappendBijRohan,
                OverlappendBijGondor,
                OverlappendMetTweeBijErebor,
                BinnenDePeriodeBijLothlorien,
                LaterBijRohan,
                BijIsengard,
                BijMoria,
                BijDale,
                BijDeAuthentiekeVereniging,
            }.Aggregate(
                new VerenigingState().Apply(_dubbele),
                (state, lid) => state.Apply(new LidmaatschapWerdToegevoegd(_dubbele.VCode, lid))
            )
        );
        session.WithVereniging(
            new[]
            {
                AuthentiekBijRohan,
                AuthentiekBijGondor,
                AuthentiekBijEreborEerst,
                AuthentiekBijEreborDaarna,
                AuthentiekBijLothlorien,
            }.Aggregate(
                new VerenigingState().Apply(_authentieke),
                (state, lid) => state.Apply(new LidmaatschapWerdToegevoegd(_authentieke.VCode, lid))
            )
        );

        var command = new MarkeerAlsDubbelVanCommand(VCode.Create(_dubbele.VCode), VCode.Create(_authentieke.VCode));

        await new MarkeerAlsDubbelVanCommandHandler(
            session,
            new VerenigingsStateQueriesMock(
                verwijderdeVerenigingen: [Isengard],
                dubbeleVerenigingen: [Moria],
                gestopteVerenigingen: [Dale]
            ),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, Fixture.Create<CommandMetadata>()));

        return session
            .SavedInvocations[command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Single();
    }

    [Fact]
    public async ValueTask Then_An_Overlapping_Lidmaatschap_Gets_The_Widest_Period_And_Keeps_The_Authentieke_Gegevens()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Lidmaatschappen.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.Samengevoegd(
                    $"Lidmaatschap bij 'Rohan' ({Rohan}) werd samengevoegd met lidmaatschap met id 1; de geldigheidsperiode werd verruimd.",
                    OverlappendBijRohan,
                    AuthentiekBijRohan with
                    {
                        DatumVan = new DateOnly(2019, 6, 1),
                        DatumTot = new DateOnly(2021, 12, 31),
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_An_Open_Start_And_An_Open_End_Stay_Open()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Lidmaatschappen.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.Samengevoegd(
                    $"Lidmaatschap bij 'Gondor' ({Gondor}) werd samengevoegd met lidmaatschap met id 2; de geldigheidsperiode werd verruimd.",
                    OverlappendBijGondor,
                    AuthentiekBijGondor with
                    {
                        DatumVan = null,
                        DatumTot = null,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_A_Lidmaatschap_Without_Overlap_Is_Toegevoegd_With_A_New_Id()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Lidmaatschappen.Should()
            .ContainEquivalentOf(
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Lidmaatschap>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    $"Lidmaatschap bij 'Rohan' ({Rohan}) werd toegevoegd vanuit de dubbele vereniging.",
                    LaterBijRohan,
                    LaterBijRohan with
                    {
                        LidmaatschapId = 6,
                    }
                )
            );
        samengevoegd.OvergenomenGegevens.Lidmaatschappen.Should().HaveCount(3);
    }

    [Fact]
    public async ValueTask Then_The_Other_Lidmaatschappen_Are_Not_Overgenomen_With_A_Reden()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .NietOvergenomenGegevens.Lidmaatschappen.Should()
            .BeEquivalentTo(
                [
                    SamenvoegingDubbeleVereniging.NietOvergenomen(
                        $"Lidmaatschap bij 'Erebor' ({Erebor})",
                        OverlappendMetTweeBijErebor,
                        LidmaatschappenSamenvoeging.RedenOverlapMetMeerdere
                    ),
                    SamenvoegingDubbeleVereniging.Gededupliceerd(
                        $"Lidmaatschap bij 'Lothlórien' ({Lothlorien})",
                        BinnenDePeriodeBijLothlorien
                    ),
                    SamenvoegingDubbeleVereniging.NietOvergenomen(
                        $"Lidmaatschap bij 'Isengard' ({Isengard})",
                        BijIsengard,
                        LidmaatschappenSamenvoeging.RedenVerwijderdeVereniging
                    ),
                    SamenvoegingDubbeleVereniging.NietOvergenomen(
                        $"Lidmaatschap bij 'Moria' ({Moria})",
                        BijMoria,
                        LidmaatschappenSamenvoeging.RedenDubbeleVereniging
                    ),
                    SamenvoegingDubbeleVereniging.NietOvergenomen(
                        $"Lidmaatschap bij 'Dale' ({Dale})",
                        BijDale,
                        LidmaatschappenSamenvoeging.RedenGestopteVereniging
                    ),
                    SamenvoegingDubbeleVereniging.NietOvergenomen(
                        $"Lidmaatschap bij 'Authentiek' ({_authentieke.VCode})",
                        BijDeAuthentiekeVereniging,
                        LidmaatschappenSamenvoeging.RedenEigenVereniging
                    ),
                ],
                options => options.WithStrictOrdering()
            );
    }

    internal static Registratiedata.Lidmaatschap Lid(
        int id,
        VCode andereVereniging,
        string naam,
        DateOnly? van,
        DateOnly? tot
    ) => new(id, andereVereniging, naam, van, tot, Identificatie: "", Beschrijving: "");
}

/// <summary>
///     De authentieke vereniging is subvereniging van Gondor; een lidmaatschap bij Gondor wordt niet overgenomen.
/// </summary>
public class Given_Lidmaatschappen_And_The_Authentieke_Is_Subvereniging
{
    [Fact]
    public async ValueTask Then_A_Lidmaatschap_Bij_The_Hoofdvereniging_Is_Not_Overgenomen()
    {
        var fixture = new Fixture().CustomizeDomain();
        var gondor = fixture.Create<VCode>();

        var authentieke = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty);

        var dubbele = fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties([])
            .WithVertegenwoordigers([]);

        var bijGondor = Given_Lidmaatschappen.Lid(1, gondor, "Gondor", null, null);

        var session = new MultipleVerenigingAggregateSessionMock(
            new VerenigingState().Apply(dubbele).Apply(new LidmaatschapWerdToegevoegd(dubbele.VCode, bijGondor))
        );
        session.WithVereniging(
            new VerenigingState()
                .Apply(authentieke)
                .Apply(
                    new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                        authentieke.VCode,
                        new Registratiedata.SubverenigingVan(gondor, "Gondor", "", "")
                    )
                )
        );

        var command = new MarkeerAlsDubbelVanCommand(VCode.Create(dubbele.VCode), VCode.Create(authentieke.VCode));

        await new MarkeerAlsDubbelVanCommandHandler(
            session,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>()));

        var samengevoegd = session
            .SavedInvocations[command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Single();

        samengevoegd.OvergenomenGegevens.Lidmaatschappen.Should().BeEmpty();
        samengevoegd
            .NietOvergenomenGegevens.Lidmaatschappen.Should()
            .BeEquivalentTo([
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    $"Lidmaatschap bij 'Gondor' ({gondor})",
                    bijGondor,
                    LidmaatschappenSamenvoeging.RedenSubvereniging
                ),
            ]);
    }
}
#pragma warning restore CS0618
