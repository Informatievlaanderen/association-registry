namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
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

/// <summary>
///     Vertegenwoordigers die enkel op de dubbele vereniging bestaan, worden toegevoegd. Deduplicatie gebeurt op INSZ.
///     De authentieke vereniging heeft Frodo (id 1) als primaire vertegenwoordiger. De dubbele vereniging heeft ook Frodo,
///     met andere gegevens, Aragorn als primaire vertegenwoordiger en Samwise.
/// </summary>
#pragma warning disable CS0618
public class Given_Vertegenwoordigers
{
    private static readonly Registratiedata.Vertegenwoordiger FrodoBijDeDubbeleVereniging = Frodo with
    {
        Email = "frodo@shire.example",
        Roepnaam = "Mr. Baggins",
    };

    private static readonly Registratiedata.Vertegenwoordiger AragornAlsPrimair = Aragorn with { IsPrimair = true };

    private static Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan() =>
        Given_Vertegenwoordigers_Helper.MarkeerAlsDubbelVan(
            dubbele: [FrodoBijDeDubbeleVereniging, AragornAlsPrimair, Samwise],
            authentieke: [Frodo]
        );

    [Fact]
    public async ValueTask Then_A_Vertegenwoordiger_With_The_Same_Insz_Is_Gededupliceerd()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .NietOvergenomenGegevens.Vertegenwoordigers.Should()
            .BeEquivalentTo([
                SamenvoegingDubbeleVereniging.Gededupliceerd("Vertegenwoordiger met id 1", FrodoBijDeDubbeleVereniging),
            ]);
    }

    [Fact]
    public async ValueTask Then_A_Primaire_Vertegenwoordiger_Becomes_NietPrimair_When_The_Authentieke_Has_A_Primaire()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Vertegenwoordigers.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "Vertegenwoordiger met id 2 werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primaire vertegenwoordiger heeft.",
                    AragornAlsPrimair,
                    Aragorn with
                    {
                        VertegenwoordigerId = 2,
                        IsPrimair = false,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_A_Vertegenwoordiger_That_Only_Exists_On_The_Dubbele_Is_Toegevoegd_With_A_New_Id()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd
            .OvergenomenGegevens.Vertegenwoordigers.Should()
            .ContainEquivalentOf(
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Vertegenwoordiger met id 3 werd toegevoegd vanuit de dubbele vereniging.",
                    Samwise,
                    Samwise with
                    {
                        VertegenwoordigerId = 3,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_The_Existing_Vertegenwoordiger_Is_Not_Overwritten()
    {
        var samengevoegd = await MarkeerAlsDubbelVan();

        samengevoegd.OvergenomenGegevens.Vertegenwoordigers.Should().HaveCount(2);
        samengevoegd
            .OvergenomenGegevens.Vertegenwoordigers.Select(x => x.NieuweWaarde)
            .Should()
            .NotContain(x => x.Insz == Frodo.Insz || x.VertegenwoordigerId == Frodo.VertegenwoordigerId);
    }
}

/// <summary>De authentieke vereniging heeft nog geen primaire vertegenwoordiger.</summary>
public class Given_Vertegenwoordigers_Without_Primaire_Vertegenwoordiger
{
    [Fact]
    public async ValueTask Then_A_Primaire_Vertegenwoordiger_Stays_Primair()
    {
        var aragornAlsPrimair = Aragorn with { IsPrimair = true };

        var samengevoegd = await Given_Vertegenwoordigers_Helper.MarkeerAlsDubbelVan(
            dubbele: [aragornAlsPrimair],
            authentieke: [Frodo with { IsPrimair = false }]
        );

        samengevoegd
            .OvergenomenGegevens.Vertegenwoordigers.Should()
            .BeEquivalentTo([
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Vertegenwoordiger met id 2 werd toegevoegd vanuit de dubbele vereniging.",
                    aragornAlsPrimair,
                    aragornAlsPrimair with
                    {
                        VertegenwoordigerId = 2,
                    }
                ),
            ]);
    }
}

internal static class Given_Vertegenwoordigers_Helper
{
    public static async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> MarkeerAlsDubbelVan(
        Registratiedata.Vertegenwoordiger[] dubbele,
        Registratiedata.Vertegenwoordiger[] authentieke
    )
    {
        var fixture = new Fixture().CustomizeDomain();

        var dubbeleVereniging = fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties([])
            .WithVertegenwoordigers(dubbele);

        var authentiekeVereniging = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithVertegenwoordigers(authentieke);

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
