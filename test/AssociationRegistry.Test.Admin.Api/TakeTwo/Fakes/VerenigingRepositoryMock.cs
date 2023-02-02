namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Fakes;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.VCodes;
using AssociationRegistry.Vereniging;
using FluentAssertions;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private readonly Vereniging? _verenigingToLoad;

    private record InvocationSave(Vereniging Vereniging);

    private record InvocationLoad(VCode VCode);

    private readonly List<InvocationSave> _invocationsSave = new();
    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingRepositoryMock(Vereniging? verenigingToLoad = null)
    {
        _verenigingToLoad = verenigingToLoad;
    }

    public async Task<StreamActionResult> Save(Vereniging vereniging, CommandMetadata metadata)
    {
        _invocationsSave.Add(new InvocationSave(vereniging));
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
        _invocationsSave.Should().HaveCount(1);
        _invocationsSave[0].Vereniging.UncommittedEvents.Should()
            .BeEquivalentTo(events, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }

    public void ShouldNotHaveAnySaves()
    {
        _invocationsSave[0].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
