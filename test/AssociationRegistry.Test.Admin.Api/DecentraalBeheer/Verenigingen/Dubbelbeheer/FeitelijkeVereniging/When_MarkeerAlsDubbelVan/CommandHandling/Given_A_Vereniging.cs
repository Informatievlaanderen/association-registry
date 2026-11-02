namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     De dubbele vereniging heeft basisgegevens en geen hoofdactiviteiten, contactgegevens, locaties of vertegenwoordigers, de authentieke vereniging heeft geen
///     basisgegevens: alle basisgegevens worden overgenomen.
///     De andere regels van de samenvoeging worden getest in <see cref="Given_Basisgegevens" /> en
///     <see cref="Given_Hoofdactiviteiten_En_Werkingsgebieden" /> , <see cref="Given_Contactgegevens" /> en
///     <see cref="Given_Locaties" /> en <see cref="Given_Vertegenwoordigers" />.
/// </summary>
#pragma warning disable CS0618
public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty);

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(
            new VerenigingState().Apply(
                _scenario
                    .FeitelijkeVerenigingWerdGeregistreerd.WithBasisgegevens(TestBasisgegevens.Fellowship)
                    .WithHoofdactiviteiten([])
                    .WithContactgegevens([])
                    .WithLocaties([])
                    .WithVertegenwoordigers([])
            )
        );
        _aggregateSessionMock.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        );

        _command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = VCode.Create(authentiekeVereniging.VCode),
        };
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>())
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingWerdGemarkeerdAlsDubbelVan_Event()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, _command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_The_Authentieke_Vereniging_Aanvaardt_The_Dubbel()
    {
        await Handle();

        SavedEventsOfAuthentiekeVereniging()
            .OfType<VerenigingAanvaarddeDubbeleVereniging>()
            .Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new VerenigingAanvaarddeDubbeleVereniging(_command.VCodeAuthentiekeVereniging, _scenario.VCode)
            );
    }

    [Fact]
    public async ValueTask Then_The_Basisgegevens_Of_The_Dubbele_Vereniging_Are_Samengevoegd()
    {
        await Handle();

        SavedEventsOfAuthentiekeVereniging()
            .OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
                    _command.VCodeAuthentiekeVereniging,
                    _scenario.VCode,
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
    public async ValueTask Then_Both_Verenigingen_Are_Saved_In_One_Transaction()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSavedTransactional(_scenario.VCode, _command.VCodeAuthentiekeVereniging);
    }

    private IEvent[] SavedEventsOfAuthentiekeVereniging() =>
        _aggregateSessionMock
            .SavedInvocations[_command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.ToArray();
}
#pragma warning restore CS0618
