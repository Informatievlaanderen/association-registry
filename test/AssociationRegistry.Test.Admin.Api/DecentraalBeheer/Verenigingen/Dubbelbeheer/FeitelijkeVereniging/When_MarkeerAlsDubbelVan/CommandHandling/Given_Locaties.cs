namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
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
using static AssociationRegistry.Test.Common.DubbelMerge.TestLocaties;

/// <summary>
///     Locaties van de dubbele vereniging worden overgenomen als ze valide en uniek zijn.
///     De authentieke vereniging heeft een primaire locatie 'Rivendell' (id 1) en een correspondentieadres (id 2).
/// </summary>
#pragma warning disable CS0618
public class Given_Locaties
{
    [Fact]
    public async ValueTask Then_A_Primaire_Locatie_Becomes_NietPrimair_When_The_Authentieke_Has_A_Primaire_Locatie()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Locaties.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "Locatie 'Bag End' werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primaire locatie heeft.",
                    LokaalFellowship,
                    LokaalFellowship with
                    {
                        LocatieId = 3,
                        IsPrimair = false,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_A_New_Activiteitenlocatie_Is_Toegevoegd()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Locaties.Should()
            .ContainEquivalentOf(
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Locatie>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Locatie 'The Prancing Pony' werd toegevoegd vanuit de dubbele vereniging.",
                    TweedeLokaalFellowship,
                    TweedeLokaalFellowship with
                    {
                        LocatieId = 4,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_The_Same_Adres_Written_Differently_Is_Gededupliceerd()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .NietOvergenomenGegevens.Locaties.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.Gededupliceerd("Locatie 'Rivendell'", RivendellBijFellowship)
            );
    }

    [Fact]
    public async ValueTask Then_A_Second_Correspondentieadres_Is_NietOvergenomen()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .NietOvergenomenGegevens.Locaties.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Locatie 'Postadres'",
                    PostadresFellowship,
                    LocatiesSamenvoeging.RedenCorrespondentie
                )
            );
    }

    [Fact]
    public async ValueTask Then_An_Invalid_Locatie_Is_NietOvergenomen()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .NietOvergenomenGegevens.Locaties.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Locatie 'Mordor'",
                    OnvolledigeLocatieFellowship,
                    LocatiesSamenvoeging.RedenNietValide
                )
            );
    }

    [Fact]
    public async ValueTask Then_Nothing_Else_Is_Merged_And_The_Existing_Locaties_Are_Untouched()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd.OvergenomenGegevens.Locaties.Should().HaveCount(2);
        samengevoegd.NietOvergenomenGegevens.Locaties.Should().HaveCount(3);
        samengevoegd
            .OvergenomenGegevens.Locaties.Select(x => x.NieuweWaarde.LocatieId)
            .Should()
            .NotIntersectWith([LokaalRivendell.LocatieId, PostadresRivendell.LocatieId]);
    }

    private static Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan() =>
        Given_Locaties_Helper.MarkeerAlsDubbelVan(
            dubbele:
            [
                LokaalFellowship,
                RivendellBijFellowship,
                PostadresFellowship,
                OnvolledigeLocatieFellowship,
                TweedeLokaalFellowship,
            ],
            authentieke: [LokaalRivendell, PostadresRivendell]
        );
}

/// <summary>De authentieke vereniging heeft nog geen primaire locatie en nog geen correspondentieadres.</summary>
public class Given_Locaties_Without_Primaire_Locatie_Or_Correspondentieadres
{
    [Fact]
    public async ValueTask Then_A_Primaire_Locatie_Stays_Primair_And_The_Correspondentieadres_Is_Toegevoegd()
    {
        var samengevoegd = await Given_Locaties_Helper.MarkeerAlsDubbelVan(
            dubbele: [LokaalFellowship, PostadresFellowship],
            authentieke: [LokaalRivendell with { IsPrimair = false }]
        );

        samengevoegd
            .OvergenomenGegevens.Locaties.Should()
            .BeEquivalentTo([
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Locatie>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Locatie 'Bag End' werd toegevoegd vanuit de dubbele vereniging.",
                    LokaalFellowship,
                    LokaalFellowship with
                    {
                        LocatieId = 2,
                    }
                ),
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Locatie>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Locatie 'Postadres' werd toegevoegd vanuit de dubbele vereniging.",
                    PostadresFellowship,
                    PostadresFellowship with
                    {
                        LocatieId = 3,
                    }
                ),
            ]);
    }
}

/// <summary>
///     Een locatie met hetzelfde adresId heeft hetzelfde adres, ook als de adresvelden anders zijn.
///     Ze is enkel een duplicaat als ook de naam en het locatietype gelijk zijn.
/// </summary>
public class Given_Locaties_With_The_Same_AdresId
{
    private static readonly Registratiedata.AdresId AdresIdRivendell = new(
        Adresbron.AR.Code,
        $"{AdresId.DataVlaanderenAdresPrefix}1234"
    );

    private static readonly Registratiedata.Locatie RivendellMetAdresId = LokaalRivendell with
    {
        IsPrimair = false,
        AdresId = AdresIdRivendell,
    };

    [Fact]
    public async ValueTask Then_A_Locatie_With_Other_Adresvelden_Is_Gededupliceerd()
    {
        var rivendellAnderAdres = RivendellMetAdresId with
        {
            LocatieId = 1,
            Adres = new Registratiedata.Adres("Hoogstraat", "99", "", "8000", "Brugge", "België"),
        };

        var samengevoegd = await Given_Locaties_Helper.MarkeerAlsDubbelVan(
            dubbele: [rivendellAnderAdres],
            authentieke: [RivendellMetAdresId]
        );

        samengevoegd.OvergenomenGegevens.Locaties.Should().BeEmpty();
        samengevoegd
            .NietOvergenomenGegevens.Locaties.Should()
            .BeEquivalentTo([SamenvoegingDubbeleVereniging.Gededupliceerd("Locatie 'Rivendell'", rivendellAnderAdres)]);
    }

    [Fact]
    public async ValueTask Then_A_Locatie_With_Another_Naam_Is_Toegevoegd()
    {
        var lothlorien = RivendellMetAdresId with { LocatieId = 1, Naam = "Lothlórien" };

        var samengevoegd = await Given_Locaties_Helper.MarkeerAlsDubbelVan(
            dubbele: [lothlorien],
            authentieke: [RivendellMetAdresId]
        );

        samengevoegd.NietOvergenomenGegevens.Locaties.Should().BeEmpty();
        samengevoegd
            .OvergenomenGegevens.Locaties.Should()
            .BeEquivalentTo([
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Locatie>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Locatie 'Lothlórien' werd toegevoegd vanuit de dubbele vereniging.",
                    lothlorien,
                    lothlorien with
                    {
                        LocatieId = 2,
                    }
                ),
            ]);
    }
}

internal static class Given_Locaties_Helper
{
    public static async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan(
        Registratiedata.Locatie[] dubbele,
        Registratiedata.Locatie[] authentieke
    )
    {
        var fixture = new Fixture().CustomizeDomain();

        var dubbeleVereniging = fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties(dubbele)
            .WithVertegenwoordigers([]);

        var authentiekeVereniging = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithLocaties(authentieke);

        var session = new MultipleVerenigingAggregateSessionMock(new VerenigingState().Apply(dubbeleVereniging));
        session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        var command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );

        await new MarkeerAlsDubbelVanCommandHandler(
            session,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>()));

        return session
            .SavedInvocations[command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Single();
    }
}
#pragma warning restore CS0618
