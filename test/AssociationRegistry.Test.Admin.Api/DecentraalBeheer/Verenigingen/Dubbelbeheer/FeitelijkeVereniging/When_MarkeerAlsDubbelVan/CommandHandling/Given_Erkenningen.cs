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
///     Erkenningen worden enkel toegevoegd als er geen erkenning bestaat voor hetzelfde IPDC-product en dezelfde
///     initiator met een overlappende periode. De authentieke vereniging heeft een verlopen erkenning (id 1) en is dus
///     niet erkend. De dubbele vereniging heeft dezelfde erkenning, een overlappende erkenning en een actieve
///     erkenning voor een ander IPDC-product.
/// </summary>
#pragma warning disable CS0618
public class Given_Erkenningen
{
    private static readonly ErkenningWerdGeregistreerd ErkenningAuthentiek = TestErkenningen.LembasBakkenRivendell;
    private static readonly ErkenningWerdGeregistreerd ZelfdeErkenningBijDeDubbeleVereniging =
        TestErkenningen.LembasBakkenRivendell;
    private static readonly ErkenningWerdGeregistreerd OverlappendeErkenning =
        TestErkenningen.OverlappendLembasBakkenRivendell;
    private static readonly ErkenningWerdGeregistreerd NieuweErkenning = TestErkenningen.RingdragersBegeleidenGondor;

    private readonly Fixture _fixture = new Fixture().CustomizeDomain();
    private readonly MultipleVerenigingAggregateSessionMock _session;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_Erkenningen()
    {
        var dubbeleVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties([])
            .WithVertegenwoordigers([])
            .WithBankrekeningnummers([]);

        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty);

        _session = new MultipleVerenigingAggregateSessionMock(
            new VerenigingState()
                .Apply(dubbeleVereniging)
                .Apply(ZelfdeErkenningBijDeDubbeleVereniging)
                .Apply(OverlappendeErkenning)
                .Apply(NieuweErkenning)
        );
        _session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging).Apply(ErkenningAuthentiek));

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
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>()));

        return _session
            .SavedInvocations[_command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.ToArray();
    }

    private async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> HandleSamengevoegd() =>
        (await Handle()).OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>().Single();

    [Fact]
    public async ValueTask Then_A_New_Erkenning_Is_Toegevoegd_With_A_New_Id()
    {
        var samengevoegd = await HandleSamengevoegd();

        var origineel = NieuweErkenning.ToSamenvoeging();

        samengevoegd
            .OvergenomenGegevens.Erkenningen.Should()
            .BeEquivalentTo([
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.Erkenning>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Erkenning 'Ringdragers begeleiden' werd toegevoegd vanuit de dubbele vereniging.",
                    origineel,
                    origineel with
                    {
                        ErkenningId = 2,
                    }
                ),
            ]);
    }

    [Fact]
    public async ValueTask Then_An_Identical_Erkenning_Is_Gededupliceerd_And_An_Overlapping_Erkenning_Is_Not_Overgenomen()
    {
        var samengevoegd = await HandleSamengevoegd();

        samengevoegd
            .NietOvergenomenGegevens.Erkenningen.Should()
            .BeEquivalentTo([
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Erkenning 'Lembas bakken'",
                    ZelfdeErkenningBijDeDubbeleVereniging.ToSamenvoeging()
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Erkenning 'Lembas bakken'",
                    OverlappendeErkenning.ToSamenvoeging(),
                    ErkenningenSamenvoeging.RedenOverlap
                ),
            ]);
    }

    [Fact]
    public async ValueTask Then_The_Overgenomen_Actieve_Erkenning_Makes_The_Vereniging_Erkend()
    {
        var events = await Handle();

        events.OfType<VerenigingWerdErkend>().Should().ContainSingle();
    }
}
#pragma warning restore CS0618
