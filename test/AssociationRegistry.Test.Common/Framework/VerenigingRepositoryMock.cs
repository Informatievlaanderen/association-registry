namespace AssociationRegistry.Test.Common.Framework;

using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using EventStore;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Vereniging;
using Xunit;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private VerenigingState? _verenigingToLoad;
    private readonly bool _expectedLoadingDubbel;
    private bool _actualLoadingDubbel;

    public record SaveInvocation(VerenigingsBase Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(string key, Type Type);
    public List<SaveInvocation> SaveInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(VerenigingState? verenigingToLoad = null, bool expectedLoadingDubbel = false)
    {
        _verenigingToLoad = verenigingToLoad;
        _expectedLoadingDubbel = expectedLoadingDubbel;
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        SaveInvocations.Add(new SaveInvocation(vereniging));

        if (_verenigingToLoad is null) return await Task.FromResult(StreamActionResult.Empty);

        foreach (var e in vereniging.UncommittedEvents)
        {
            _verenigingToLoad = _verenigingToLoad.Apply((dynamic)e);
        }

        return await Task.FromResult(StreamActionResult.Empty);
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => await Save(vereniging, metadata, cancellationToken);

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _actualLoadingDubbel = allowDubbeleVereniging;
        _invocationsLoad.Add(new InvocationLoad(vCode, typeof(TVereniging)));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingToLoad!);

        return await Task.FromResult(vereniging);
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, long? expectedVersion)
    {
        _invocationsLoad.Add(new InvocationLoad(kboNummer, typeof(VerenigingMetRechtspersoonlijkheid)));

        if (_verenigingToLoad is null) throw new AggregateNotFoundException(kboNummer, typeof(VerenigingMetRechtspersoonlijkheid));

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.Hydrate(_verenigingToLoad);

        return await Task.FromResult(vereniging);
    }

    public Task<bool> IsVerwijderd(VCode vCode)
        => Task.FromResult(false);

    public Task<bool> IsDubbel(VCode vCode)
        => Task.FromResult(false);

    public async Task<bool> Exists(VCode vCode)
        => true;

    public void ShouldHaveLoaded<TVereniging>(params string[] keys) where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Should().BeEquivalentTo(
            keys.Select(key => new InvocationLoad(key, typeof(TVereniging))),
            config: options => options.WithStrictOrdering());
    }

    public void ShouldHaveSaved(params IEvent[] events)
    {
        SaveInvocations.Should().HaveCount(1);

        SaveInvocations[0].Vereniging.UncommittedEvents.ToArray().ShouldCompare(events);

        AssertLoadingDubbel();
    }

    public void AssertLoadingDubbel()
    {
        _actualLoadingDubbel.Should().Be(_expectedLoadingDubbel);

    }

    public void ShouldNotHaveSaved<TEvent>() where TEvent : IEvent
    {
        SaveInvocations[0].Vereniging.UncommittedEvents.Should().NotContain(e => e.GetType() == typeof(TEvent));
    }

    public void ShouldNotHaveAnySaves()
    {
        SaveInvocations[0].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
