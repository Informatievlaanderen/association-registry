namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Alba;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using TestClasses;

public interface IApiSetup
{
    public HttpClient SuperAdminHttpClient { get; }
    public HttpClient UnautenticatedClient { get; }
    public HttpClient UnauthorizedClient { get; }
    public IAlbaHost AdminApiHost { get; }
    public IAlbaHost AcmApiHost { get; }
    public IAlbaHost AdminProjectionHost { get; }
    public IAlbaHost PublicProjectionHost { get; }
    public IAlbaHost PublicApiHost { get; }

    public IProjectionDaemon AdminProjectionDaemon { get; }

    Task<Dictionary<string, IEvent[]>> ExecuteGiven(IScenario scenario, IDocumentSession session);
    Task RefreshIndices();
}
