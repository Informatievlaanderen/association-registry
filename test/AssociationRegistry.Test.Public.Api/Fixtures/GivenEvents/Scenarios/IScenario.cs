namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Vereniging;

public interface IScenario
{
    public VCode VCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}
