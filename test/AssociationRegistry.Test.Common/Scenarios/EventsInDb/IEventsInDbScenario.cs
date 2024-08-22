namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using EventStore;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}
