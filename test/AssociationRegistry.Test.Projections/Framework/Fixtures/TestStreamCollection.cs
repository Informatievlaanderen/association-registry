namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using AssociationRegistry.Framework;
using Events;
using JasperFx.Core;
using System.Collections;

public sealed record TestStreamCollection : IEnumerable<TestStreamDefinition>
{
    private readonly List<TestStreamDefinition> _streamDefinitions = new();

    public void Add(string streamKey, params IEvent[] events)
    {
        var streamDefinition = _streamDefinitions.SingleOrDefault(sod => sod.StreamKey.Equals(streamKey));

        if (streamDefinition is not null)
        {
            _streamDefinitions.Remove(streamDefinition);

            events = Array.Empty<IEvent>()
                          .AddRange(streamDefinition.Events)
                          .AddRange(events)
                          .ToArray();
        }

        _streamDefinitions.Add(new TestStreamDefinition(streamKey, events));
    }

    public IEnumerator<TestStreamDefinition> GetEnumerator() => _streamDefinitions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed record TestStreamDefinition(string StreamKey, IReadOnlyCollection<IEvent> Events);
