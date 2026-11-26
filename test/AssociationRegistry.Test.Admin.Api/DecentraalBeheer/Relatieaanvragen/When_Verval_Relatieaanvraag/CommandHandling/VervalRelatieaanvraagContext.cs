namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Verval_Relatieaanvraag.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using static Verenigingen;

/// <summary>
///     A relatieaanvraag between <see cref="KoepelA" /> and <see cref="VerenigingC" />, ingediend on
///     <see cref="AanvraagDatum" />, for which the job sends a command on the given day. The repository refuses to look up
///     the open relatieaanvragen: verval loads no vereniging and checks no rule of the type.
/// </summary>
public class VervalRelatieaanvraagContext
{
    public static readonly DateOnly AanvraagDatum = new(2026, 8, 20);
    public static readonly DateOnly VervalDatum = AanvraagDatum.AddDays(90);
    public static readonly RelatieaanvraagId AanvraagId = RelatieaanvraagId.Create(1);

    private readonly RelatieaanvraagRepositoryStub _repository;
    private readonly VervalRelatieaanvraagCommandHandler _commandHandler;

    public VervalRelatieaanvraagContext(
        DateOnly today,
        RelatieType? relatieType = null,
        bool alreadyBevestigd = false,
        bool alreadyGeweigerd = false,
        bool alreadyIngetrokken = false,
        bool alreadyVervallen = false
    )
    {
        var ingediend = new RelatieaanvraagWerdIngediend(
            AanvraagId.Value,
            KoepelA.Value,
            VerenigingC.Value,
            KoepelA.Value,
            (relatieType ?? RelatieType.Lidmaatschap).Value,
            "Aanvraag",
            AanvraagDatum,
            VervalDatum
        );

        var relatieaanvraag = new RelatieaanvraagState().Apply(ingediend) with { Version = 1 };

        if (alreadyBevestigd)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdBevestigd(AanvraagId.Value, AanvraagDatum));

        if (alreadyGeweigerd)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdGeweigerd(AanvraagId.Value, AanvraagDatum, "Reden")
            );

        if (alreadyIngetrokken)
            relatieaanvraag = relatieaanvraag.Apply(
                new RelatieaanvraagWerdIngetrokken(AanvraagId.Value, AanvraagDatum, "Reden")
            );

        if (alreadyVervallen)
            relatieaanvraag = relatieaanvraag.Apply(new RelatieaanvraagWerdVervallen(AanvraagId.Value, VervalDatum));

        _repository = new RelatieaanvraagRepositoryStub(relatieaanvraag);

        _commandHandler = new VervalRelatieaanvraagCommandHandler(
            _repository,
            new ClockStub(today),
            NullLogger<VervalRelatieaanvraagCommandHandler>.Instance
        );
    }

    public RelatieaanvraagWerdVervallen? Vervallen => _repository.Afsluiting as RelatieaanvraagWerdVervallen;

    public Task Verval() =>
        _commandHandler.Handle(
            new CommandEnvelope<VervalRelatieaanvraagCommand>(
                new VervalRelatieaanvraagCommand(AanvraagId.Value),
                CommandMetadata.ForDigitaalVlaanderenProcess
            )
        );

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
            aanvraagId == _relatieaanvraag.AanvraagId && expectedVersion is null
                ? Task.FromResult(_relatieaanvraag)
                : throw new NotSupportedException();

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
