namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using AutoFixture;
using FluentAssertions;
using static Verenigingen;

/// <summary>
///     A relatieaanvraag between <see cref="KoepelA" /> and <see cref="VerenigingC" />, ingediend on
///     <see cref="AanvraagDatum" />, that is withdrawn on <see cref="Today" /> unless another day is given. The handler
///     gets no aggregate session, and the repository refuses to look up the open relatieaanvragen: withdrawing loads no
///     vereniging and checks no rule again.
/// </summary>
public class TrekRelatieaanvraagInContext
{
    public static readonly DateOnly AanvraagDatum = new(2026, 11, 2);
    public static readonly DateOnly Today = new(2026, 11, 24);
    public static readonly RelatieaanvraagId AanvraagId = RelatieaanvraagId.Create(1);

    private readonly RelatieaanvraagRepositoryStub _repository;
    private readonly TrekRelatieaanvraagInCommandHandler _commandHandler;

    public TrekRelatieaanvraagInContext(
        VCode aangevraagdDoor,
        RelatieType? relatieType = null,
        bool alreadyBevestigd = false,
        bool alreadyGeweigerd = false,
        bool alreadyIngetrokken = false,
        bool alreadyVervallen = false,
        DateOnly? today = null
    )
    {
        var ingediend = new RelatieaanvraagWerdIngediend(
            AanvraagId.Value,
            KoepelA.Value,
            VerenigingC.Value,
            aangevraagdDoor.Value,
            (relatieType ?? RelatieType.Lidmaatschap).Value,
            "Aanvraag",
            AanvraagDatum,
            AanvraagDatum.AddDays(90)
        );

        var relatieaanvraag = new RelatieaanvraagState().Apply(ingediend) with { Version = 1 };

        if (alreadyBevestigd)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdBevestigd(AanvraagId.Value, AanvraagDatum));

        if (alreadyGeweigerd)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdGeweigerd(AanvraagId.Value, AanvraagDatum));

        if (alreadyIngetrokken)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdIngetrokken(AanvraagId.Value, AanvraagDatum)
            );

        if (alreadyVervallen)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdVervallen(AanvraagId.Value, ingediend.VervalDatum.AddDays(1))
            );

        _repository = new RelatieaanvraagRepositoryStub(relatieaanvraag);
        _commandHandler = new TrekRelatieaanvraagInCommandHandler(_repository, new ClockStub(today ?? Today));
    }

    public RelatieaanvraagWerdIngetrokken? Ingetrokken => _repository.Afsluiting as RelatieaanvraagWerdIngetrokken;

    public async Task<RelatieaanvraagCommandResult> TrekIn(VCode ingetrokkenDoor)
    {
        var command = new TrekRelatieaanvraagInCommand(AanvraagId, ingetrokkenDoor);

        var metadata = new Fixture().CustomizeAdminApi().Create<CommandMetadata>() with { ExpectedVersion = null };

        return await _commandHandler.Handle(new CommandEnvelope<TrekRelatieaanvraagInCommand>(command, metadata));
    }

    public void ShouldNotHaveSaved() => _repository.Afsluiting.Should().BeNull();

    private class RelatieaanvraagRepositoryStub : IRelatieaanvraagRepository
    {
        private readonly RelatieaanvraagState _relatieaanvraag;

        public RelatieaanvraagRepositoryStub(RelatieaanvraagState relatieaanvraag)
        {
            _relatieaanvraag = relatieaanvraag;
        }

        public IRelatieaanvraagEvent? Afsluiting { get; private set; }

        public Task<RelatieaanvraagState> Load(RelatieaanvraagId aanvraagId, long? expectedVersion) =>
            Task.FromResult(_relatieaanvraag);

        public Task<StreamActionResult> SaveAfsluiting(
            RelatieaanvraagState relatieaanvraag,
            IRelatieaanvraagEvent afsluiting,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        )
        {
            Afsluiting = afsluiting;

            return Task.FromResult(new StreamActionResult(Sequence: 10, Version: relatieaanvraag.Version + 1));
        }

        public Task<OpenRelatieaanvragen> GetOpenRelatieaanvragen(
            VCode koepelVCode,
            VCode verenigingVCode,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();

        public Task<StreamActionResult> SaveNew(
            Relatieaanvraag relatieaanvraag,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();

        public Task<StreamActionResult> SaveBevestiging(
            RelatieaanvraagState relatieaanvraag,
            RelatieaanvraagWerdBevestigd bevestigd,
            VerenigingsBase vereniging,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();
    }
}
