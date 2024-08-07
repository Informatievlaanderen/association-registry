namespace AssociationRegistry.Test.Admin.Api.Projections.Scenarios;

using AssociationRegistry.Framework;
using Vereniging;

public interface IScenario
{
    VCode VCode { get; }
    IEvent[] CreateEvents();
}
