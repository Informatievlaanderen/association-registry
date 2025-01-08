namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using Vereniging;

public interface IScenario
{
    public VCode VCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}
