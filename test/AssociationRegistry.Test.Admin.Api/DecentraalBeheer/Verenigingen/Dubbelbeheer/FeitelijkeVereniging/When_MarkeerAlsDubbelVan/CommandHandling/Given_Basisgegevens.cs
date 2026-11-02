namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     De basisgegevens van de dubbele vereniging worden enkel overgenomen als ze op de authentieke vereniging leeg zijn.
///     Een bestaande waarde op de authentieke vereniging wordt nooit overschreven.
/// </summary>
#pragma warning disable CS0618
public class Given_Basisgegevens
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    [Fact]
    public async ValueTask Given_An_Empty_Authentieke_Vereniging_Then_All_Basisgegevens_Are_Overgenomen()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbele: TestBasisgegevens.Fellowship,
            authentieke: TestBasisgegevens.Empty
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                new OvergenomenBasisgegevens(
                    KorteNaam: Verrijkt(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam),
                    KorteBeschrijving: Verrijkt(
                        Velden.KorteBeschrijving,
                        TestBasisgegevens.Fellowship.KorteBeschrijving
                    ),
                    Startdatum: Verrijkt(Velden.Startdatum, TestBasisgegevens.Fellowship.Startdatum!.Value),
                    Doelgroep: Verrijkt(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep)
                ),
                NietOvergenomenBasisgegevens.Geen
            )
        );
    }

    [Fact]
    public async ValueTask Given_Both_Verenigingen_Have_Basisgegevens_Then_The_Authentieke_Values_Are_Kept()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbele: TestBasisgegevens.Fellowship,
            authentieke: TestBasisgegevens.Rivendell
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                OvergenomenBasisgegevens.Geen,
                new NietOvergenomenBasisgegevens(
                    KorteNaam: NietOvergenomen(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam),
                    KorteBeschrijving: NietOvergenomen(
                        Velden.KorteBeschrijving,
                        TestBasisgegevens.Fellowship.KorteBeschrijving
                    ),
                    Startdatum: NietOvergenomen(Velden.Startdatum, TestBasisgegevens.Fellowship.Startdatum!.Value),
                    Doelgroep: NietOvergenomen(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep)
                )
            )
        );
    }

    [Fact]
    public async ValueTask Given_Both_Verenigingen_Have_The_Same_Basisgegevens_Then_They_Are_Gededupliceerd()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbele: TestBasisgegevens.Fellowship,
            authentieke: TestBasisgegevens.Fellowship
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                OvergenomenBasisgegevens.Geen,
                new NietOvergenomenBasisgegevens(
                    KorteNaam: Gededupliceerd(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam),
                    KorteBeschrijving: Gededupliceerd(
                        Velden.KorteBeschrijving,
                        TestBasisgegevens.Fellowship.KorteBeschrijving
                    ),
                    Startdatum: Gededupliceerd(Velden.Startdatum, TestBasisgegevens.Fellowship.Startdatum!.Value),
                    Doelgroep: Gededupliceerd(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep)
                )
            )
        );
    }

    [Fact]
    public async ValueTask Given_Only_Some_Basisgegevens_On_The_Authentieke_Vereniging_Then_Only_The_Empty_Ones_Are_Overgenomen()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbele: TestBasisgegevens.Fellowship,
            authentieke: TestBasisgegevens.Empty with
            {
                KorteNaam = TestBasisgegevens.Rivendell.KorteNaam,
                Doelgroep = TestBasisgegevens.Rivendell.Doelgroep,
            }
        );

        session.ShouldHaveSaved(
            authentieke,
            new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele),
            Samengevoegd(
                authentieke,
                dubbele,
                OvergenomenBasisgegevens.Geen with
                {
                    KorteBeschrijving = Verrijkt(
                        Velden.KorteBeschrijving,
                        TestBasisgegevens.Fellowship.KorteBeschrijving
                    ),
                    Startdatum = Verrijkt(Velden.Startdatum, TestBasisgegevens.Fellowship.Startdatum!.Value),
                },
                NietOvergenomenBasisgegevens.Geen with
                {
                    KorteNaam = NietOvergenomen(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam),
                    Doelgroep = NietOvergenomen(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep),
                }
            )
        );
    }

    [Fact]
    public async ValueTask Given_An_Empty_Dubbele_Vereniging_Then_Nothing_Is_Samengevoegd()
    {
        var (session, dubbele, authentieke) = await MarkeerAlsDubbelVan(
            dubbele: TestBasisgegevens.Empty,
            authentieke: TestBasisgegevens.Rivendell
        );

        session.ShouldHaveSaved(authentieke, new VerenigingAanvaarddeDubbeleVereniging(authentieke, dubbele));
    }

    private async Task<(
        MultipleVerenigingAggregateSessionMock Session,
        VCode Dubbele,
        VCode Authentieke
    )> MarkeerAlsDubbelVan(TestBasisgegevens dubbele, TestBasisgegevens authentieke)
    {
        var dubbeleVereniging = _fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(dubbele)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([]);
        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(authentieke);

        var session = new MultipleVerenigingAggregateSessionMock(new VerenigingState().Apply(dubbeleVereniging));
        session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        var command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );

        await new MarkeerAlsDubbelVanCommandHandler(session, new VerenigingsStateQueriesMock()).Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())
        );

        return (session, command.VCode, command.VCodeAuthentiekeVereniging);
    }

    private static DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd(
        VCode authentieke,
        VCode dubbele,
        OvergenomenBasisgegevens overgenomen,
        NietOvergenomenBasisgegevens nietOvergenomen
    ) =>
        EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            authentieke,
            dubbele,
            overgenomen,
            nietOvergenomen
        );
}
#pragma warning restore CS0618
