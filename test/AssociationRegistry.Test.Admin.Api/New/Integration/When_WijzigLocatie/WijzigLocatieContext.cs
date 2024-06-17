namespace AssociationRegistry.Test.Admin.Api.New;

using Alba;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using Framework;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Namotion.Reflection;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;
using System.Net;
using System.Security.Claims;
using Vereniging;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;
using AdresId = AssociationRegistry.Admin.Api.Verenigingen.Common.AdresId;

[CollectionDefinition(nameof(WijzigLocatieContext))]
public class WijzigLocatieContextCollection : ICollectionFixture<AppFixture>
{

}
public abstract class WijzigLocatieContext : IAsyncLifetime
{
    private IScenarioResult _result;
    private readonly AppFixture _fixture;
    private readonly IDocumentStore ProjectionStore;
    public WijzigLocatieRequest Request { get; private set; }
    public V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields Scenario { get; private set; }

    protected WijzigLocatieContext(AppFixture fixture)
    {
        _fixture = fixture;
        Host = fixture.Host;
        Store = Host.Services.GetRequiredService<IDocumentStore>();
        ProjectionStore = fixture.ProjectionHostServer.Services.GetRequiredService<IDocumentStore>();
    }

    public IAlbaHost Host { get; }
    public IDocumentStore Store { get; }

    public async Task InitializeAsync()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        // Using Marten, wipe out all data and reset the state
        await Store.Advanced.ResetAllData();
        await Given(new V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields());

        Request = new WijzigLocatieRequest()
        {
            Locatie =
                new TeWijzigenLocatie()
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "9200",
                        Gemeente = "Dendermonde",
                        Land = "Belgie",
                    },
                    IsPrimair = true,
                    Locatietype = Locatietype.Correspondentie,
                }
        };

        _result = await Host.Scenario(s =>
        {
            s.Patch
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/{Scenario.VCode}/locaties/{Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        var daemon = await ProjectionStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(15));
    }

    private async Task Given(V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields scenario)
    {
        Scenario = scenario;

        await using (var session = Store.LightweightSession())
        {
            session.SetHeader(MetadataHeaderNames.Initiator, "metadata.Initiator");
            session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
            session.CorrelationId = Guid.NewGuid().ToString();

            session.Events.Append(Scenario.VCode, Scenario.GetEvents());
            await session.SaveChangesAsync();
        }
    }

    public string ResultingVCode { get; set; }

    // This is required because of the IAsyncLifetime
    // interface. Note that I do *not* tear down database
    // state after the test. That's purposeful
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
