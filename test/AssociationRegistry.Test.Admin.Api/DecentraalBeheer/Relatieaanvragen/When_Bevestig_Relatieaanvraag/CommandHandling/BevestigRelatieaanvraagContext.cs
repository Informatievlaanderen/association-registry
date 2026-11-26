namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.MartenDb.Store;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using AutoFixture;
using FluentAssertions;
using Moq;

/// <summary>
///     A relatieaanvraag between the koepel and the vereniging, ingediend on <see cref="AanvraagDatum" />, that is
///     confirmed on <see cref="Today" /> unless another day is given.
/// </summary>
public class BevestigRelatieaanvraagContext
{
    public static readonly DateOnly AanvraagDatum = new(2026, 11, 2);
    public static readonly DateOnly Today = new(2026, 11, 23);
    public static readonly RelatieaanvraagId AanvraagId = RelatieaanvraagId.Create(1);

    private readonly Mock<IAggregateSession> _aggregateSession = new();
    private readonly RelatieaanvraagRepositoryStub _repository;
    private readonly BevestigRelatieaanvraagCommandHandler _commandHandler;

    /// <param name="otherOpenRelatieaanvragen">Open relatieaanvragen of the vereniging with other koepels.</param>
    public BevestigRelatieaanvraagContext(
        VerenigingState koepel,
        VerenigingState vereniging,
        RelatieType relatieType,
        VCode aangevraagdDoor,
        OpenRelatieaanvraag[]? otherOpenRelatieaanvragen = null,
        bool alreadyBevestigd = false,
        bool alreadyGeweigerd = false,
        bool alreadyIngetrokken = false,
        bool alreadyVervallen = false,
        DateOnly? today = null
    )
    {
        SetupLoad(koepel);
        SetupLoad(vereniging);

        var ingediend = new RelatieaanvraagWerdIngediend(
            AanvraagId.Value,
            koepel.VCode.Value,
            vereniging.VCode.Value,
            aangevraagdDoor.Value,
            relatieType.Value,
            "Aanvraag",
            AanvraagDatum,
            AanvraagDatum.AddDays(90)
        );

        var relatieaanvraag = new RelatieaanvraagState().Apply(ingediend) with { Version = 1 };

        if (alreadyBevestigd)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdBevestigd(AanvraagId.Value, Today));

        if (alreadyGeweigerd)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdGeweigerd(AanvraagId.Value, Today, "Reden"));

        if (alreadyIngetrokken)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdIngetrokken(AanvraagId.Value, Today, "Reden")
            );

        if (alreadyVervallen)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdVervallen(AanvraagId.Value, ingediend.VervalDatum)
            );

        _repository = new RelatieaanvraagRepositoryStub(
            relatieaanvraag,
            OpenRelatieaanvragenIncludingItself(relatieType, otherOpenRelatieaanvragen ?? [])
        );

        _commandHandler = new BevestigRelatieaanvraagCommandHandler(
            _aggregateSession.Object,
            _repository,
            new ClockStub(today ?? Today)
        );
    }

    public RelatieaanvraagWerdBevestigd? Bevestigd => _repository.Bevestigd;
    public IReadOnlyCollection<IEvent> VerenigingEvents => _repository.VerenigingEvents;

    public async Task<RelatieaanvraagCommandResult> Bevestig(
        VCode bevestigdDoor,
        Relatiegegevens? relatiegegevens = null
    )
    {
        var command = new BevestigRelatieaanvraagCommand(
            AanvraagId,
            bevestigdDoor,
            relatiegegevens ?? Relatiegegevens.Empty
        );

        var metadata = new Fixture().CustomizeAdminApi().Create<CommandMetadata>() with { ExpectedVersion = null };

        return await _commandHandler.Handle(new CommandEnvelope<BevestigRelatieaanvraagCommand>(command, metadata));
    }

    public void ShouldNotHaveSaved() => _repository.Bevestigd.Should().BeNull();

    public void ShouldNotHaveLoadedTheVerenigingen() =>
        _aggregateSession.Verify(
            x =>
                x.Load<VerenigingOfAnyKind>(
                    It.IsAny<VCode>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                ),
            Times.Never
        );

    /// <summary>
    ///     What the repository returns: the relatieaanvraag itself is still open, so it holds the key of the koepel and
    ///     the vereniging, and it is the open subverenigingsaanvraag or one of the open lidmaatschapsaanvragen.
    /// </summary>
    private static OpenRelatieaanvragen OpenRelatieaanvragenIncludingItself(
        RelatieType relatieType,
        OpenRelatieaanvraag[] others
    )
    {
        var itself = new OpenRelatieaanvraag(AanvraagId, relatieType);
        var otherLidmaatschapsaanvragen = others.Where(other => !other.RelatieType.IsSubvereniging).ToArray();

        if (relatieType.IsSubvereniging)
            return new OpenRelatieaanvragen(itself, itself, otherLidmaatschapsaanvragen);

        var otherSubverenigingsaanvraag = others.SingleOrDefault(other => other.RelatieType.IsSubvereniging);

        return new OpenRelatieaanvragen(itself, otherSubverenigingsaanvraag, [itself, .. otherLidmaatschapsaanvragen]);
    }

    private void SetupLoad(VerenigingState state) =>
        _aggregateSession
            .Setup(x =>
                x.Load<VerenigingOfAnyKind>(
                    state.VCode,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(() =>
            {
                var vereniging = new VerenigingOfAnyKind();
                vereniging.Hydrate(state);

                return vereniging;
            });

    private class RelatieaanvraagRepositoryStub : IRelatieaanvraagRepository
    {
        private readonly RelatieaanvraagState _relatieaanvraag;
        private readonly OpenRelatieaanvragen _openRelatieaanvragen;

        public RelatieaanvraagRepositoryStub(
            RelatieaanvraagState relatieaanvraag,
            OpenRelatieaanvragen openRelatieaanvragen
        )
        {
            _relatieaanvraag = relatieaanvraag;
            _openRelatieaanvragen = openRelatieaanvragen;
        }

        public RelatieaanvraagWerdBevestigd? Bevestigd { get; private set; }
        public IReadOnlyCollection<IEvent> VerenigingEvents { get; private set; } = [];

        public Task<RelatieaanvraagState> Load(RelatieaanvraagId aanvraagId, long? expectedVersion) =>
            Task.FromResult(_relatieaanvraag);

        public Task<OpenRelatieaanvragen> GetOpenRelatieaanvragen(
            VCode koepelVCode,
            VCode verenigingVCode,
            CancellationToken cancellationToken
        ) => Task.FromResult(_openRelatieaanvragen);

        public Task<StreamActionResult> SaveBevestiging(
            RelatieaanvraagState relatieaanvraag,
            RelatieaanvraagWerdBevestigd bevestigd,
            VerenigingsBase vereniging,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        )
        {
            Bevestigd = bevestigd;
            VerenigingEvents = vereniging.UncommittedEvents.ToArray();

            return Task.FromResult(new StreamActionResult(Sequence: 10, Version: relatieaanvraag.Version + 1));
        }

        public Task<StreamActionResult> SaveNew(
            Relatieaanvraag relatieaanvraag,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();

        public Task<StreamActionResult> SaveAfsluiting(
            RelatieaanvraagState relatieaanvraag,
            IRelatieaanvraagEvent afsluiting,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        ) => throw new NotSupportedException();
    }
}
