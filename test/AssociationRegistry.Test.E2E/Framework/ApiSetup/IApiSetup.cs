namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Alba;
using TestClasses;

public interface IApiSetup
{
    public IAlbaHost AdminApiHost { get; }
    public IAlbaHost AcmApiHost { get; }
    public IAlbaHost AdminProjectionHost { get; }
    public IAlbaHost PublicProjectionHost { get; }
    public IAlbaHost PublicApiHost { get; }
    Task ExecuteGiven(IScenario scenario);
}
