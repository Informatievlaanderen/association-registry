namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Vereniging;

public interface IScenario
{
    public VCode AfdelingVCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}
