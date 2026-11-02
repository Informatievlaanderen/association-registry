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
using static AssociationRegistry.Test.Common.DubbelMerge.TestContactgegevens;

/// <summary>
///     Contactgegevens van de dubbele vereniging worden overgenomen als ze valide en uniek zijn.
///     De authentieke vereniging heeft een primaire e-mail (id 1) en een telefoonnummer (id 2).
///     De dubbele vereniging heeft een primaire e-mail, een primaire website, een social media link, hetzelfde
///     telefoonnummer anders geschreven en een ongeldig telefoonnummer.
/// </summary>
#pragma warning disable CS0618
public class Given_Contactgegevens
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();
    private readonly MultipleVerenigingAggregateSessionMock _session;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_Contactgegevens()
    {
        var dubbeleVereniging = _fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([
                EmailFellowship,
                WebsiteFellowship,
                TelefoonFellowship,
                OngeldigeTelefoonFellowship,
                SocialMediaFellowship,
            ])
            .WithLocaties([])
            .WithVertegenwoordigers([]);

        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithContactgegevens([EmailRivendell, TelefoonRivendell]);

        _session = new MultipleVerenigingAggregateSessionMock(new VerenigingState().Apply(dubbeleVereniging));
        _session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        _command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );
    }

    private async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> Handle()
    {
        await new MarkeerAlsDubbelVanCommandHandler(
            _session,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>()));

        return _session
            .SavedInvocations[_command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Single();
    }

    [Fact]
    public async ValueTask Then_A_Primair_Contactgegeven_Becomes_NietPrimair_When_The_Authentieke_Has_A_Primair_Of_That_Type()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .OvergenomenGegevens.Contactgegevens.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "E-mail 'info@fellowship.example' werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primair contactgegeven van dit type heeft.",
                    EmailFellowship,
                    EmailFellowship with
                    {
                        ContactgegevenId = 3,
                        IsPrimair = false,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_A_Primair_Contactgegeven_Stays_Primair_When_The_Authentieke_Has_No_Primair_Of_That_Type()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .OvergenomenGegevens.Contactgegevens.Should()
            .ContainEquivalentOf(
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Contactgegeven>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Website 'https://fellowship.example' werd toegevoegd vanuit de dubbele vereniging.",
                    WebsiteFellowship,
                    WebsiteFellowship with
                    {
                        ContactgegevenId = 4,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_A_NietPrimair_Contactgegeven_Is_Toegevoegd()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .OvergenomenGegevens.Contactgegevens.Should()
            .ContainEquivalentOf(
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Contactgegeven>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "SocialMedia 'https://social.example/fellowship' werd toegevoegd vanuit de dubbele vereniging.",
                    SocialMediaFellowship,
                    SocialMediaFellowship with
                    {
                        ContactgegevenId = 5,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask Then_The_Same_Telefoonnummer_Written_Differently_Is_Gededupliceerd()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .NietOvergenomenGegevens.Contactgegevens.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.Gededupliceerd("Telefoon '+32 11 22 33 44'", TelefoonFellowship)
            );
    }

    [Fact]
    public async ValueTask Then_An_Invalid_Contactgegeven_Is_NietOvergenomen()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .NietOvergenomenGegevens.Contactgegevens.Should()
            .ContainEquivalentOf(
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Telefoon 'geen nummer'",
                    OngeldigeTelefoonFellowship,
                    ContactgegevensSamenvoeging.RedenNietValide
                )
            );
    }

    [Fact]
    public async ValueTask Then_Nothing_Else_Is_Merged_And_The_Existing_Contactgegevens_Are_Untouched()
    {
        var samengevoegd = await Handle();

        samengevoegd.OvergenomenGegevens.Contactgegevens.Should().HaveCount(3);
        samengevoegd.NietOvergenomenGegevens.Contactgegevens.Should().HaveCount(2);
        samengevoegd
            .OvergenomenGegevens.Contactgegevens.Select(x => x.NieuweWaarde.ContactgegevenId)
            .Should()
            .NotIntersectWith([EmailRivendell.ContactgegevenId, TelefoonRivendell.ContactgegevenId]);
    }
}
#pragma warning restore CS0618
