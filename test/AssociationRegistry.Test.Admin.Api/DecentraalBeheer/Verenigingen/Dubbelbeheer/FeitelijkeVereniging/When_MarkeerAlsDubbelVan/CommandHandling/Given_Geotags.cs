namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;
using static AssociationRegistry.Test.Common.DubbelMerge.TestLocaties;
using static AssociationRegistry.Test.Common.DubbelMerge.TestWerkingsgebieden;

/// <summary>
///     De samenvoeging kan locaties en werkingsgebieden toevoegen, dus de geotags van de authentieke vereniging
///     worden daarna opnieuw berekend.
/// </summary>
#pragma warning disable CS0618
public class Given_Geotags
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();
    private readonly MultipleVerenigingAggregateSessionMock _session;
    private readonly Mock<IGeotagsService> _geotagsService;
    private readonly GeotagsCollection _geotags;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_Geotags()
    {
        var dubbeleVereniging = _fixture
            .Create<FeitelijkeVerenigingWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties([LokaalFellowship]);

        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithLocaties([LokaalRivendell]);

        _session = new MultipleVerenigingAggregateSessionMock(
            new VerenigingState()
                .Apply(dubbeleVereniging)
                .Apply(new WerkingsgebiedenWerdenBepaald(dubbeleVereniging.VCode, [VlaamsBrabant]))
        );
        _session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        (_geotagsService, _geotags) = new GeotagsServiceFactory(_fixture).ReturnsRandomGeotags();

        _command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );
    }

    private async Task<IEvent[]> Handle()
    {
        await new MarkeerAlsDubbelVanCommandHandler(
            _session,
            new VerenigingsStateQueriesMock(),
            _geotagsService.Object
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>()));

        return _session
            .SavedInvocations[_command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.ToArray();
    }

    [Fact]
    public async ValueTask Then_The_Geotags_Are_Calculated_With_The_Overgenomen_Locaties_And_Werkingsgebieden()
    {
        await Handle();

        _geotagsService.Verify(
            x =>
                x.CalculateGeotags(
                    It.Is<IEnumerable<Locatie>>(locaties =>
                        locaties
                            .Select(l => l.Naam)
                            .OrderBy(naam => naam)
                            .SequenceEqual(new[] { LokaalFellowship.Naam, LokaalRivendell.Naam }.OrderBy(naam => naam))
                    ),
                    It.Is<IEnumerable<Werkingsgebied>>(werkingsgebieden =>
                        werkingsgebieden.Select(w => w.Code).SequenceEqual(new[] { VlaamsBrabant.Code })
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async ValueTask Then_GeotagsWerdenBepaald_Is_Saved_After_The_Samenvoeging()
    {
        var events = await Handle();

        events
            .Select(x => x.GetType())
            .Should()
            .Equal(
                typeof(VerenigingAanvaarddeDubbeleVereniging),
                typeof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging),
                typeof(GeotagsWerdenBepaald)
            );

        events
            .OfType<GeotagsWerdenBepaald>()
            .Single()
            .Should()
            .BeEquivalentTo(EventFactory.GeotagsWerdenBepaald(_command.VCodeAuthentiekeVereniging, _geotags));
    }
}
#pragma warning restore CS0618
