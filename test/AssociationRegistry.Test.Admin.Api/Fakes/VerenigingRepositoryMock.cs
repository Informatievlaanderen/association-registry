namespace AssociationRegistry.Test.Admin.Api.Fakes;

using EventStore;
using AssociationRegistry.Framework;
using Vereniging;
using FluentAssertions;
using IEvent = AssociationRegistry.Framework.IEvent;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private readonly Vereniging? _verenigingToLoad;

    public record SaveInvocation(Vereniging Vereniging);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(VCode VCode);

    public List<SaveInvocation> SaveInvocations { get; } = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(Vereniging? verenigingToLoad = null)
    {
        _verenigingToLoad = verenigingToLoad;
    }

    public async Task<StreamActionResult> Save(Vereniging vereniging, CommandMetadata metadata)
    {
        SaveInvocations.Add(new SaveInvocation(vereniging));
        return await Task.FromResult(StreamActionResult.Empty);
    }

    public async Task<Vereniging> Load(VCode vCode, long? expectedVersion)
    {
        _invocationsLoad.Add(new InvocationLoad(vCode));
        return (await Task.FromResult(_verenigingToLoad))!;
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

    public void ShouldNotHaveAnySaves()
    {
        SaveInvocations[0].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
