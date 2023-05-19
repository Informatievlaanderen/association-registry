namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using EventStore;
using AssociationRegistry.Framework;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}
