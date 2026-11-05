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
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;
using Velden = AssociationRegistry.Events.SamenvoegingDubbeleVereniging.Velden;

/// <summary>
///     Is de authentieke vereniging een vereniging met rechtspersoonlijkheid, dan worden de korte naam, de startdatum en
///     de vertegenwoordigers niet overgenomen, want die worden enkel via de KBO bijgewerkt. De gegevens die zo'n
///     vereniging zelf mag beheren, worden wel samengevoegd.
///     De authentieke vereniging heeft geen korte naam, startdatum, korte beschrijving of doelgroep. De dubbele vereniging
///     heeft alle basisgegevens, Frodo en Samwise als vertegenwoordigers en een website.
/// </summary>
#pragma warning disable CS0618
public class Given_VerenigingMetRechtspersoonlijkheid
{
    private const string Reden = VerenigingMetRechtspersoonlijkheidSamenvoeging.RedenBeheerdViaKbo;

    [Fact]
    public async ValueTask Then_The_KorteNaam_And_Startdatum_Are_Not_Overgenomen_Even_When_Empty()
    {
        var samengevoegd = await Given_VerenigingMetRechtspersoonlijkheid_Helper.MarkeerAlsDubbelVan(
            korteNaamAuthentiek: ""
        );

        samengevoegd.OvergenomenGegevens.Basisgegevens.KorteNaam.Should().BeNull();
        samengevoegd.OvergenomenGegevens.Basisgegevens.Startdatum.Should().BeNull();

        samengevoegd
            .NietOvergenomenGegevens.Basisgegevens.KorteNaam.Should()
            .BeEquivalentTo(
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    Velden.KorteNaam,
                    TestBasisgegevens.Fellowship.KorteNaam,
                    Reden
                )
            );
        samengevoegd
            .NietOvergenomenGegevens.Basisgegevens.Startdatum.Should()
            .BeEquivalentTo(
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    Velden.Startdatum,
                    TestBasisgegevens.Fellowship.Startdatum!.Value,
                    Reden
                )
            );
    }

    [Fact]
    public async ValueTask Then_The_KorteBeschrijving_And_Doelgroep_Are_Still_Verrijkt()
    {
        var samengevoegd = await Given_VerenigingMetRechtspersoonlijkheid_Helper.MarkeerAlsDubbelVan(
            korteNaamAuthentiek: ""
        );

        samengevoegd
            .OvergenomenGegevens.Basisgegevens.KorteBeschrijving.Should()
            .BeEquivalentTo(
                SamenvoegingDubbeleVereniging.Verrijkt(
                    Velden.KorteBeschrijving,
                    TestBasisgegevens.Fellowship.KorteBeschrijving
                )
            );
        samengevoegd
            .OvergenomenGegevens.Basisgegevens.Doelgroep.Should()
            .BeEquivalentTo(
                SamenvoegingDubbeleVereniging.Verrijkt(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep)
            );
    }

    [Fact]
    public async ValueTask Then_The_Same_KorteNaam_Is_Gededupliceerd()
    {
        var samengevoegd = await Given_VerenigingMetRechtspersoonlijkheid_Helper.MarkeerAlsDubbelVan(
            korteNaamAuthentiek: TestBasisgegevens.Fellowship.KorteNaam
        );

        samengevoegd
            .NietOvergenomenGegevens.Basisgegevens.KorteNaam.Should()
            .BeEquivalentTo(
                SamenvoegingDubbeleVereniging.Gededupliceerd(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam)
            );
    }

    [Fact]
    public async ValueTask Then_No_Vertegenwoordiger_Is_Overgenomen()
    {
        var samengevoegd = await Given_VerenigingMetRechtspersoonlijkheid_Helper.MarkeerAlsDubbelVan(
            korteNaamAuthentiek: ""
        );

        samengevoegd.OvergenomenGegevens.Vertegenwoordigers.Should().BeEmpty();
        samengevoegd
            .NietOvergenomenGegevens.Vertegenwoordigers.Should()
            .BeEquivalentTo([
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    $"Vertegenwoordiger met id {Frodo.VertegenwoordigerId}",
                    new SamenvoegingDubbeleVereniging.NietOvergenomenVertegenwoordiger(Frodo.VertegenwoordigerId),
                    Reden
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    $"Vertegenwoordiger met id {Samwise.VertegenwoordigerId}",
                    new SamenvoegingDubbeleVereniging.NietOvergenomenVertegenwoordiger(Samwise.VertegenwoordigerId),
                    Reden
                ),
            ]);
    }

    [Fact]
    public async ValueTask Then_The_Contactgegevens_Are_Still_Overgenomen()
    {
        var samengevoegd = await Given_VerenigingMetRechtspersoonlijkheid_Helper.MarkeerAlsDubbelVan(
            korteNaamAuthentiek: ""
        );

        samengevoegd
            .OvergenomenGegevens.Contactgegevens.Select(x => x.OrigineleWaarde)
            .Should()
            .BeEquivalentTo([TestContactgegevens.WebsiteFellowship]);
    }
}

internal static class Given_VerenigingMetRechtspersoonlijkheid_Helper
{
    public static async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan(
        string korteNaamAuthentiek
    )
    {
        var fixture = new Fixture().CustomizeDomain();

        var dubbeleVereniging = fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Fellowship)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([TestContactgegevens.WebsiteFellowship])
            .WithLocaties([])
            .WithVertegenwoordigers([Frodo, Samwise]);

        var authentiekeVereniging = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            KorteNaam = korteNaamAuthentiek,
            Startdatum = null,
        };

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
