namespace AssociationRegistry.Test.Admin.Api.Fakes;

using AssociationRegistry.Framework;
using EventStore;
using FluentAssertions;
using Vereniging;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private VerenigingState? _verenigingToLoad;
    private readonly VerenigingsRepository.VCodeAndNaam _moederVCodeAndNaam;
    public record SaveInvocation(VerenigingsBase Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(VCode VCode, Type Type);
    public List<SaveInvocation> SaveInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(VerenigingState? verenigingToLoad = null, VerenigingsRepository.VCodeAndNaam moederVCodeAndNaam = null!)
    {
        _verenigingToLoad = verenigingToLoad;
        _moederVCodeAndNaam = moederVCodeAndNaam;
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

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Add(new InvocationLoad(vCode, typeof(TVereniging)));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingToLoad!);

        return await Task.FromResult(vereniging);
    }

    public Task<VerenigingsRepository.VCodeAndNaam?> GetVCodeAndNaam(KboNummer kboNummer)
        => Task.FromResult(_moederVCodeAndNaam)!;

    public void ShouldHaveLoaded<TVereniging>(params string[] vCodes) where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Should().BeEquivalentTo(
            vCodes.Select(vCode => new InvocationLoad(VCode.Create(vCode), typeof(TVereniging))),
            config: options => options.WithStrictOrdering());
    }

    public void ShouldHaveSaved(params IEvent[] events)
    {
        SaveInvocations.Should().HaveCount(1);

        SaveInvocations[0].Vereniging.UncommittedEvents.Should()
                          .BeEquivalentTo(events, config: options => options.RespectingRuntimeTypes().WithStrictOrdering());
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
