namespace AssociationRegistry.Test.E2E.Scenarios;

using AssociationRegistry.Framework;
using Framework.TestClasses;
using Vereniging;

public class EmptyScenario: IScenario
{
    public VCode VCode { get; }

    public IEvent[] CreateEvents()
        => [];

    public CommandMetadata GetCommandMetadata()
        => null;
}
