namespace AssociationRegistry.Test.Admin.Api.Fakes;

using EventStore;
using AssociationRegistry.Framework;
using Vereniging;
using FluentAssertions;
using IEvent = AssociationRegistry.Framework.IEvent;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private VerenigingState? _verenigingToLoad;

    public record SaveInvocation(VerenigingsBase Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(VCode VCode);

    public List<SaveInvocation> SaveInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(VerenigingState? verenigingToLoad = null)
    {
        _verenigingToLoad = verenigingToLoad;
    }

    public async Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken = default)
    {
        SaveInvocations.Add(new SaveInvocation(vereniging));

        if (_verenigingToLoad is not null)
            foreach (var e in vereniging.UncommittedEvents)
            {
                _verenigingToLoad = _verenigingToLoad.Apply((dynamic)e);
            }

        return await Task.FromResult(StreamActionResult.Empty);
    }

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion) where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad.Add(new InvocationLoad(vCode));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingToLoad!);
        return await Task.FromResult(vereniging);
    }

    public void ShouldHaveLoaded(params string[] vCodes)
    {
        _invocationsLoad.Should().BeEquivalentTo(
            vCodes.Select(vCode => new InvocationLoad(VCode.Create(vCode))),
            options => options.WithStrictOrdering());
    }

    public void ShouldHaveSaved(params IEvent[] events)
    {
        SaveInvocations.Should().HaveCount(1);
        SaveInvocations[0].Vereniging.UncommittedEvents.Should()
            .BeEquivalentTo(events, options => options.RespectingRuntimeTypes().WithStrictOrdering());
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
