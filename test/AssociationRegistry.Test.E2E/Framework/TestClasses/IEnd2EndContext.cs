namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;
using ApiSetup;
using AssociationRegistry.Framework;
using Vereniging;

public interface IEnd2EndContext<TRequest>
{
    string VCode { get; }
    TRequest Request { get; }
    IAlbaHost AdminApiHost { get; }
    IAlbaHost QueryApiHost { get; }
}

public interface IScenario
{
    string VCode { get; }
    Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService service);
    Task WhenCommand(FullBlownApiSetup setup);
}
