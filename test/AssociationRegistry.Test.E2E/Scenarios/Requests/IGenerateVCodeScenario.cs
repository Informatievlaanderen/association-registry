namespace AssociationRegistry.Test.E2E.Scenarios.Commands;

using AssociationRegistry.Framework;
using Events;
using Framework.TestClasses;
using Vereniging;

public interface IGenerateVCodeScenario: IScenario
{
    VCode VCode { get; }
}

public interface IVerenigingWerdGeregistreerdScenario
{
    FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
}
