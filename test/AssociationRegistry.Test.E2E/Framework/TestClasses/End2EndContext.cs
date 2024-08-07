namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;
using AssociationRegistry.Framework;
using Marten;
using NodaTime;
using NodaTime.Text;

public abstract class End2EndContext<TRequest, TScenario> : IEnd2EndContext<TRequest> where TScenario : IScenario
{
    private IApiSetup _fixture;
    public string VCode => Scenario.VCode;
    public TRequest Request { get; init; }
    public IAlbaHost AdminApiHost { get; }
    public IAlbaHost QueryApiHost { get; }
    public IAlbaHost ProjectionHost { get; }
    public IAlbaHost ProjectionHost2 => _fixture.ProjectionHost;

    protected End2EndContext(IApiSetup fixture)
    {
        _fixture = fixture;
        _fixture.InitializeAsync(SchemaName)
                .GetAwaiter().GetResult();

        AdminApiHost = _fixture.AdminApiHost;
        QueryApiHost = _fixture.QueryApiHost;
        ProjectionHost = _fixture.ProjectionHost;

    }

    protected abstract string SchemaName { get; }

    protected async Task Given(TScenario scenario)
    {
        Scenario = scenario;

        await using (var session = AdminApiHost.DocumentStore().LightweightSession())
        {
            session.SetHeader(MetadataHeaderNames.Initiator, "metadata.Initiator");
            session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
            session.CorrelationId = Guid.NewGuid().ToString();

            session.Events.Append(scenario.VCode, scenario.CreateEvents());
            await session.SaveChangesAsync();
        }
    }

    public TScenario Scenario { get; set; }
}
