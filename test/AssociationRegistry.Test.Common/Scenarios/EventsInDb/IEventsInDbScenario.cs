namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using Events;
using EventStore;
using MartenDb.Store;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevensDocuments()
        => [];
    CommandMetadata GetCommandMetadata();
}
