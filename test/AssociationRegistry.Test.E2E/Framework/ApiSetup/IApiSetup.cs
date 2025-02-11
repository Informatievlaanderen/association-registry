namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Alba;
using Marten.Events.Daemon;
using TestClasses;

public interface IApiSetup
{
    public HttpClient SuperAdminHttpClient { get; }
    public IAlbaHost AdminApiHost { get; }
    public IAlbaHost AcmApiHost { get; }
    public IAlbaHost AdminProjectionHost { get; }
    public IAlbaHost PublicProjectionHost { get; }
    public IAlbaHost PublicApiHost { get; }

    public IProjectionDaemon AdminProjectionDaemon { get; }

    Task ExecuteGiven(IScenario scenario);
}
