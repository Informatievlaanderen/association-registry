namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;
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

public class DienRelatieaanvraagInContext
{
    public const string Beschrijving = "Aanvraag om lid te worden van de koepel";
    public static readonly DateOnly Vandaag = new(2026, 11, 19);
    public static readonly RelatieaanvraagId AanvraagId = RelatieaanvraagId.Create(1);

    private readonly Mock<IAggregateSession> _aggregateSession = new();
    private readonly RelatieaanvraagRepositoryStub _repository;
    private readonly RelatieaanvraagIdServiceStub _aanvraagIdService = new(AanvraagId);
    private readonly DienRelatieaanvraagInCommandHandler _commandHandler;

    public DienRelatieaanvraagInContext(
        VerenigingState koepel,
        VerenigingState vereniging,
        OpenRelatieaanvragen? openRelatieaanvragen = null
    )
    {
        Koepel = koepel;
        Vereniging = vereniging;
        Metadata = new Fixture().CustomizeAdminApi().Create<CommandMetadata>();

        SetupLoad(koepel);
        SetupLoad(vereniging);

        _repository = new RelatieaanvraagRepositoryStub(openRelatieaanvragen ?? OpenRelatieaanvragen.Geen);

        _commandHandler = new DienRelatieaanvraagInCommandHandler(
            _aggregateSession.Object,
            _repository,
            _aanvraagIdService,
            new ClockStub(Vandaag)
        );
    }

    public VerenigingState Koepel { get; }
    public VerenigingState Vereniging { get; }
    public CommandMetadata Metadata { get; }

    public DienRelatieaanvraagInCommand Command(RelatieType relatieType, VCode? aangevraagdDoor = null) =>
        new(
            Partijen.Create(Koepel.VCode, Vereniging.VCode, aangevraagdDoor ?? Koepel.VCode),
            relatieType,
            Beschrijving
        );

    public async Task<RelatieaanvraagCommandResult> Handle(DienRelatieaanvraagInCommand command) =>
        await _commandHandler.Handle(new CommandEnvelope<DienRelatieaanvraagInCommand>(command, Metadata));

    public RelatieaanvraagWerdIngediend Ingediend(RelatieType relatieType, VCode aangevraagdDoor) =>
        new(
            AanvraagId.Value,
            Koepel.VCode.Value,
            Vereniging.VCode.Value,
            aangevraagdDoor.Value,
            relatieType.Value,
            Beschrijving,
            Vandaag,
            Vandaag.AddDays(90)
        );

    /// <summary>The relatieaanvraag is of the type that was asked, with the event.</summary>
    public void ShouldHaveSaved(RelatieaanvraagWerdIngediend ingediend)
    {
        _repository.Saved.Should().ContainSingle();

        _repository
            .Saved.Single()
            .Should()
            .BeOfType(
                RelatieType.Parse(ingediend.RelatieType).IsSubvereniging
                    ? typeof(Subverenigingsaanvraag)
                    : typeof(Lidmaatschapsaanvraag)
            );

        _repository.Saved.Single().UncommittedEvents.Should().ContainSingle().Which.Should().Be(ingediend);
    }

    /// <summary>A refused relatieaanvraag is not saved and does not use an aanvraagId.</summary>
    public void ShouldNotHaveSaved()
    {
        _repository.Saved.Should().BeEmpty();
        _aanvraagIdService.Requested.Should().Be(0);
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
        private readonly OpenRelatieaanvragen _openRelatieaanvragen;

        public RelatieaanvraagRepositoryStub(OpenRelatieaanvragen openRelatieaanvragen)
        {
            _openRelatieaanvragen = openRelatieaanvragen;
        }

        public List<Relatieaanvraag> Saved { get; } = [];

        public Task<OpenRelatieaanvragen> GetOpenRelatieaanvragen(
            VCode koepelVCode,
            VCode verenigingVCode,
            CancellationToken cancellationToken
        ) => Task.FromResult(_openRelatieaanvragen);

        public Task<StreamActionResult> SaveNew(
            Relatieaanvraag relatieaanvraag,
            CommandMetadata metadata,
            CancellationToken cancellationToken
        )
        {
            Saved.Add(relatieaanvraag);

            return Task.FromResult(new StreamActionResult(Sequence: 1, Version: 1));
        }

        public Task<RelatieaanvraagState> Load(RelatieaanvraagId aanvraagId, long? expectedVersion) =>
            throw new NotSupportedException();

        public Task<StreamActionResult> SaveBevestiging(
            RelatieaanvraagState relatieaanvraag,
            RelatieaanvraagWerdBevestigd bevestigd,
            VerenigingsBase vereniging,
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

    private class RelatieaanvraagIdServiceStub : IRelatieaanvraagIdService
    {
        private readonly RelatieaanvraagId _aanvraagId;

        public RelatieaanvraagIdServiceStub(RelatieaanvraagId aanvraagId)
        {
            _aanvraagId = aanvraagId;
        }

        public int Requested { get; private set; }

        public Task<RelatieaanvraagId> GetNext()
        {
            Requested++;

            return Task.FromResult(_aanvraagId);
        }
    }
}
