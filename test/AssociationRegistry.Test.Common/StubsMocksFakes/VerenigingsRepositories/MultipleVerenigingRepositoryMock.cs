namespace AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using MartenDb.Store;

public class MultipleVerenigingRepositoryMock : IVerenigingsRepository
{
    private List<VerenigingState?> _verenigingenToLoad;
    private readonly bool _expectedLoadingDubbel;
    private bool _actualLoadingDubbel;
    private bool _expectedLoadingVerwijderd;
    private bool _actualLoadingVerwijderd;

    public record SaveInvocation(VerenigingsBase Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(string key, Type Type);
    public Dictionary<VCode, SaveInvocation[]> SavedInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public MultipleVerenigingRepositoryMock(VerenigingState? verenigingToLoad = null, bool expectedLoadingDubbel = false, bool expectedLoadingVerwijderd = false)
    {
        _verenigingenToLoad = new List<VerenigingState>() { verenigingToLoad };
        _expectedLoadingDubbel = expectedLoadingDubbel;
        _expectedLoadingVerwijderd = expectedLoadingVerwijderd;
    }

    public void WithVereniging(VerenigingState vereniging)
    {
        _verenigingenToLoad.Add(vereniging);
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var verenigingToLoad = _verenigingenToLoad.Single(x => x.VCode == vereniging.VCode);

        if (!SavedInvocations.TryGetValue(vereniging.VCode, out var invocations))
        {
            invocations = Array.Empty<SaveInvocation>(); // Initialize if not present
        }

        SavedInvocations[vereniging.VCode] = invocations.Append(new SaveInvocation(vereniging)).ToArray();

        if (verenigingToLoad is null) return await Task.FromResult(StreamActionResult.Empty);

        foreach (var e in vereniging.UncommittedEvents)
        {
            verenigingToLoad = verenigingToLoad.Apply((dynamic)e);
        }

        return await Task.FromResult(StreamActionResult.Empty);
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => await Save(vereniging, metadata, cancellationToken);

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, CommandMetadata metadata, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _actualLoadingDubbel = allowDubbeleVereniging;
        _actualLoadingVerwijderd = allowVerwijderdeVereniging;
        _invocationsLoad.Add(new InvocationLoad(vCode, typeof(TVereniging)));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingenToLoad.Single(x => x.VCode == vCode));

        return await Task.FromResult(vereniging);
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata)
    {
        _invocationsLoad.Add(new InvocationLoad(kboNummer, typeof(VerenigingMetRechtspersoonlijkheid)));

        if (_verenigingenToLoad is null) throw new AggregateNotFoundException(kboNummer, typeof(VerenigingMetRechtspersoonlijkheid));

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.Hydrate(_verenigingenToLoad.Single(x => x.KboNummer == kboNummer));

        return await Task.FromResult(vereniging);
    }

    public Task<bool> IsVerwijderd(VCode vCode)
        => Task.FromResult(false);

    public Task<bool> IsDubbel(VCode vCode)
        => Task.FromResult(false);

    public async Task<bool> Exists(VCode vCode)
        => true;

    public async Task<bool> Exists(KboNummer kboNummer)
        => throw new NotImplementedException();

    public async Task<StreamActionResult> SaveNew(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata messageMetadata, CancellationToken cancellationToken)
        => await Save(vereniging, messageMetadata, cancellationToken);

    public void ShouldHaveLoaded<TVereniging>(params string[] keys) where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Should().BeEquivalentTo(
            keys.Select(key => new InvocationLoad(key, typeof(TVereniging))),
            config: options => options.WithStrictOrdering());
    }

    public void ShouldHaveSaved(VCode vCode, params IEvent[] events)
    {
        var savedInvocations = SavedInvocations[vCode].ToList();
        savedInvocations.Should().HaveCount(1);

        savedInvocations[0].Vereniging.UncommittedEvents.ToArray().ShouldCompare(events);

        AssertLoadingDubbel();
    }

    public void AssertLoadingDubbel()
    {
        _actualLoadingDubbel.Should().Be(_expectedLoadingDubbel);
    }

    public void AssertLoadingVerwijderd()
    {
        _actualLoadingVerwijderd.Should().Be(_expectedLoadingVerwijderd);
    }

    public void ShouldNotHaveSaved<TEvent>(VCode vCode) where TEvent : IEvent
    {
        var savedInvocations = SavedInvocations[vCode].ToList();
        savedInvocations.Should().HaveCount(1);

        savedInvocations[0].Vereniging.UncommittedEvents.Should().NotContain(e => e.GetType() == typeof(TEvent));
    }

    public void ShouldNotHaveAnySaves(VCode vCode)
    {
        var savedInvocations = SavedInvocations[vCode].ToList();
        savedInvocations.Should().HaveCount(1);

        savedInvocations[0].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
