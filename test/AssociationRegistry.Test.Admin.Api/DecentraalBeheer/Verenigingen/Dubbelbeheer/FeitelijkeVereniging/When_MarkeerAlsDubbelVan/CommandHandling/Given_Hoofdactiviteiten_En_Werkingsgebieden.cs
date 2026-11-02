namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;
using static AssociationRegistry.Test.Common.DubbelMerge.TestHoofdactiviteiten;
using static AssociationRegistry.Test.Common.DubbelMerge.TestWerkingsgebieden;

/// <summary>
///     Hoofdactiviteiten en werkingsgebieden van de dubbele vereniging worden toegevoegd als de code nog niet bestaat op
///     de authentieke vereniging. Bestaande codes worden niet dubbel toegevoegd.
///     'Niet van toepassing' kan niet gecombineerd worden met andere werkingsgebieden.
/// </summary>
#pragma warning disable CS0618
public class Given_Hoofdactiviteiten_En_Werkingsgebieden
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    [Fact]
    public async ValueTask Given_A_New_Hoofdactiviteit_Then_It_Is_Toegevoegd_And_An_Existing_One_Is_Gededupliceerd()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbeleHoofdactiviteiten: [Sport, Jeugdwerk],
            authentiekeHoofdactiviteiten: [Sport]
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                Overgenomen() with
                {
                    Hoofdactiviteiten = [Toegevoegd("Hoofdactiviteit 'Jeugdwerk'", Jeugdwerk)],
                },
                NietOvergenomen() with
                {
                    Hoofdactiviteiten = [Gededupliceerd("Hoofdactiviteit 'Sport'", Sport)],
                }
            )
        );
    }

    [Fact]
    public async ValueTask Given_A_New_Werkingsgebied_Then_It_Is_Toegevoegd_And_An_Existing_One_Is_Gededupliceerd()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbeleWerkingsgebieden: [VlaamsBrabant, Antwerpen],
            authentiekeWerkingsgebieden: [VlaamsBrabant]
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                Overgenomen() with
                {
                    Werkingsgebieden = [Toegevoegd("Werkingsgebied 'Provincie Antwerpen'", Antwerpen)],
                },
                NietOvergenomen() with
                {
                    Werkingsgebieden = [Gededupliceerd("Werkingsgebied 'Provincie Vlaams-Brabant'", VlaamsBrabant)],
                }
            )
        );
    }

    [Fact]
    public async ValueTask Given_The_Authentieke_Vereniging_Is_NietVanToepassing_Then_No_Werkingsgebied_Is_Overgenomen()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbeleWerkingsgebieden: [VlaamsBrabant],
            authentiekeWerkingsgebieden: [NietVanToepassing]
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                Overgenomen(),
                NietOvergenomen() with
                {
                    Werkingsgebieden =
                    [
                        SamenvoegingDubbeleVereniging.NietOvergenomen(
                            "Werkingsgebied 'Provincie Vlaams-Brabant'",
                            VlaamsBrabant,
                            WerkingsgebiedenSamenvoeging.RedenNietVanToepassing
                        ),
                    ],
                }
            )
        );
    }

    [Fact]
    public async ValueTask Given_The_Dubbele_Vereniging_Is_NietVanToepassing_Then_It_Is_Not_Combined_With_Existing_Werkingsgebieden()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbeleWerkingsgebieden: [NietVanToepassing],
            authentiekeWerkingsgebieden: [VlaamsBrabant]
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                Overgenomen(),
                NietOvergenomen() with
                {
                    Werkingsgebieden =
                    [
                        SamenvoegingDubbeleVereniging.NietOvergenomen(
                            "Werkingsgebied 'Niet van toepassing'",
                            NietVanToepassing,
                            WerkingsgebiedenSamenvoeging.RedenNietVanToepassing
                        ),
                    ],
                }
            )
        );
    }

    [Fact]
    public async ValueTask Given_The_Dubbele_Vereniging_Is_NietVanToepassing_And_The_Authentieke_Has_No_Werkingsgebieden_Then_It_Is_Toegevoegd()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbeleWerkingsgebieden: [NietVanToepassing],
            authentiekeWerkingsgebieden: []
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                Overgenomen() with
                {
                    Werkingsgebieden = [Toegevoegd("Werkingsgebied 'Niet van toepassing'", NietVanToepassing)],
                },
                NietOvergenomen()
            )
        );
    }

    private async Task<(
        MultipleVerenigingAggregateSessionMock Session,
        VCode Dubbele,
        VCode Authentieke
    )> MarkeerAlsDubbelVan(
        Registratiedata.HoofdactiviteitVerenigingsloket[]? dubbeleHoofdactiviteiten = null,
        Registratiedata.HoofdactiviteitVerenigingsloket[]? authentiekeHoofdactiviteiten = null,
        Registratiedata.Werkingsgebied[]? dubbeleWerkingsgebieden = null,
        Registratiedata.Werkingsgebied[]? authentiekeWerkingsgebieden = null
    )
    {
        var dubbeleVereniging = _fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten(dubbeleHoofdactiviteiten ?? [])
            .WithContactgegevens([])
            .WithLocaties([])
            .WithVertegenwoordigers([]);
        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten(authentiekeHoofdactiviteiten ?? []);

        var session = new MultipleVerenigingAggregateSessionMock(
            WithWerkingsgebieden(new VerenigingState().Apply(dubbeleVereniging), dubbeleWerkingsgebieden ?? [])
        );
        session.WithVereniging(
            WithWerkingsgebieden(new VerenigingState().Apply(authentiekeVereniging), authentiekeWerkingsgebieden ?? [])
        );

        var command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );

        await new MarkeerAlsDubbelVanCommandHandler(
            session,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        return (session, command.VCode, command.VCodeAuthentiekeVereniging);
    }

    private static VerenigingState WithWerkingsgebieden(
        VerenigingState state,
        Registratiedata.Werkingsgebied[] werkingsgebieden
    ) =>
        werkingsgebieden switch
        {
            [] => state,
            [var werkingsgebied] when werkingsgebied == NietVanToepassing => state.Apply(
                new WerkingsgebiedenWerdenNietVanToepassing(state.VCode)
            ),
            _ => state.Apply(new WerkingsgebiedenWerdenBepaald(state.VCode, werkingsgebieden)),
        };

    private static OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen() =>
        OvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        >.WithOnly(OvergenomenBasisgegevens.Geen);

    private static NietOvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > NietOvergenomen() =>
        NietOvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        >.WithOnly(NietOvergenomenBasisgegevens.Geen);

    private static DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd(
        VCode authentieke,
        VCode dubbele,
        OvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        > overgenomen,
        NietOvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        > nietOvergenomen
    ) =>
        EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            authentieke,
            dubbele,
            overgenomen,
            nietOvergenomen
        );
}
#pragma warning restore CS0618
