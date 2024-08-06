namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema;
using Alba;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Framework.Scenarios;
using Framework.TestClasses;
using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Text;
using System.Net;
using Vereniging;
using Xunit;

public class WijzigLocatieContext<T> : End2EndContext<WijzigLocatieRequest, FeitelijkeVerenigingWerdGeregistreerdScenario>, IAsyncLifetime
where T: IApiSetup, new()
{
    private IScenarioResult _result;
    private IApiSetup _fixture;
    public WijzigLocatieRequest Request { get; private set; }
    public IAlbaHost AdminApiHost => _fixture.AdminApiHost;
    public IAlbaHost QueryApiHost => _fixture.QueryApiHost;

    public WijzigLocatieContext()
    {
        _fixture = new T();
        _fixture.InitializeAsync($"wijzig{GetType().GetGenericArguments().First().Name}")
                .GetAwaiter().GetResult();
    }

    public async Task InitializeAsync()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        await Given(Scenario);

        Request = new WijzigLocatieRequest()
        {
            Locatie =
                new TeWijzigenLocatie()
                {
                    Naam = "Kantoor",
                    Adres = new Admin.Api.Verenigingen.Common.Adres
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
        // Using Marten, wipe out all data and reset the state
        await _fixture.AdminApiHost.DocumentStore().Advanced.ResetAllData();

        _result = await AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/{Scenario.VCode}/locaties/{Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        VCode = _result.Context.Response.Headers.Location.First().Split('/').Last();

        await _fixture.ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public string VCode { get; set; }
    public Metadata Metadata { get; set; }

    public new Task DisposeAsync()
        => Task.CompletedTask;
}


