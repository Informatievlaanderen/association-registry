namespace AssociationRegistry.Test;

using AssociationRegistry.Framework;
using Events;
using EventStore;
using Marten;
using MartenDb.Store;
using Moq;
using System.Runtime.CompilerServices;

public static class EventStoreMockExtensions
    {
        // keeps a per-mock list of captured events (auto-collected when mock is GC'd)
        private static readonly ConditionalWeakTable<IEventStore, List<IEvent>> _captured =
            new();

        /// <summary>
        /// Set up the mock so every Save(...) call captures the passed IEvent[].
        /// </summary>
        public static Mock<IEventStore> CaptureEvents(this Mock<IEventStore> mock)
        {
            var list = new List<IEvent>();
            // reset previous storage (if any)
            _captured.Remove(mock.Object);
            _captured.Add(mock.Object, list);

            mock.Setup(x => x.Save(
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<IDocumentSession>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<IEvent[]>()))
                .Callback<string, long, IDocumentSession, CommandMetadata, CancellationToken, IEvent[]>(
                    (_, _, _, _, _, events) =>
                    {
                        if (events != null && events.Length > 0)
                            list.AddRange(events);
                    })
                .ReturnsAsync(new StreamActionResult(0, 0));

            return mock;
        }

        /// <summary>
        /// Returns all events captured so far (in call order). Empty if none.
        /// </summary>
        public static IReadOnlyList<IEvent> GetCapturedEvents(this Mock<IEventStore> mock)
            => _captured.TryGetValue(mock.Object, out var list)
                ? list
                : Array.Empty<IEvent>();

        /// <summary>
        /// Clears the captured events list for this mock.
        /// </summary>
        public static void ClearCapturedEvents(this Mock<IEventStore> mock)
        {
            if (_captured.TryGetValue(mock.Object, out var list))
                list.Clear();
        }
    }
