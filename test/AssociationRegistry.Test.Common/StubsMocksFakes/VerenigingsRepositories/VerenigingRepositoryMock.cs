namespace AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using MartenDb.Store;
using Microsoft.Extensions.DependencyInjection;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private VerenigingState? _verenigingToLoad;
    private readonly bool _expectedLoadingDubbel;
    private bool _actualLoadingDubbel;
    private bool _expectedLoadingVerwijderd;
    private bool _actualLoadingVerwijderd;

    public record SaveInvocation(VerenigingsBase Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(string key, Type Type);
    public List<SaveInvocation> SaveInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(VerenigingState? verenigingToLoad = null, bool expectedLoadingDubbel = false, bool expectedLoadingVerwijderd = false)
    {
        _verenigingToLoad = verenigingToLoad;
        _expectedLoadingDubbel = expectedLoadingDubbel;
        _expectedLoadingVerwijderd = expectedLoadingVerwijderd;
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

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, CommandMetadata metadata, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _actualLoadingDubbel = allowDubbeleVereniging;
        _actualLoadingVerwijderd = allowVerwijderdeVereniging;
        _invocationsLoad.Add(new InvocationLoad(vCode, typeof(TVereniging)));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingToLoad!);

        return await Task.FromResult(vereniging);
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata)
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

    public async Task<bool> Exists(KboNummer kboNummer)
        => true;

    public async Task<StreamActionResult> SaveNew(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata messageMetadata, CancellationToken cancellationToken)
        => await Save(vereniging, messageMetadata, cancellationToken);

    public void ShouldHaveLoaded<TVereniging>(params string[] keys) where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Should().BeEquivalentTo(
            keys.Select(key => new InvocationLoad(key, typeof(TVereniging))),
            config: options => options.WithStrictOrdering());
    }

    // public void ShouldHaveSavedAtLeast(params IEvent[] expectedEvents)
    // {
    //     SaveInvocations.Should().HaveCount(1);
    //
    //     var actual = SaveInvocations[0].Vereniging.UncommittedEvents.ToArray();
    //
    //     actual.Should().Contain(expectedEvents);
    //
    //     AssertLoadingDubbel();
    // }

    public void ShouldHaveSavedExact(params IEvent[] events)
    {
        SaveInvocations.Should().HaveCount(1);

        SaveInvocations[0].Vereniging.UncommittedEvents.ToArray().ShouldCompare(events);

        AssertLoadingDubbel();
    }

    public T[] ShouldHaveSavedEventType<T>(int times)
    {
        SaveInvocations.Should().HaveCount(1);

        var events = SaveInvocations[0].Vereniging.UncommittedEvents.OfType<T>();
        events.Should().HaveCount(times);

        AssertLoadingDubbel();

        return events.ToArray();
    }

    public void ShouldNotHaveSavedEventType<T>()
    {
        SaveInvocations.Should().HaveCount(1);

        SaveInvocations[0].Vereniging.UncommittedEvents.OfType<T>().Should().HaveCount(0);

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

    public void ShouldNotHaveSaved<TEvent>() where TEvent : IEvent
    {
        SaveInvocations[0].Vereniging.UncommittedEvents.Should().NotContain(e => e.GetType() == typeof(TEvent));
    }

    public void ShouldNotHaveAnySaves()
    {
        SaveInvocations[0].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
