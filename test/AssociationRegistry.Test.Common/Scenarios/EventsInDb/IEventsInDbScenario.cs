namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}
