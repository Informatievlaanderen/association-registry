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


[CollectionDefinition(nameof(WijzigLocatieContext<AdminApiSetup>))]
public class WijzigLocatieCollection : ICollectionFixture<WijzigLocatieContext<AdminApiSetup>>
{

}

public class WijzigLocatieContext<T> : End2EndContext<WijzigLocatieRequest, FeitelijkeVerenigingWerdGeregistreerdScenario>, IAsyncLifetime
where T: IApiSetup, new()
{
    private IScenarioResult _result;

    protected override string SchemaName => $"wijzig{GetType().GetGenericArguments().First().Name}";

    public WijzigLocatieContext() : base(new T())
    {
        Scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
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
    }

    public async Task InitializeAsync()
    {
        await AdminApiHost.DocumentStore().Advanced.ResetAllData();

        await Given(Scenario);
        _result = await AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/{Scenario.VCode}/locaties/{Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public Metadata Metadata { get; set; }

    public new Task DisposeAsync()
        => Task.CompletedTask;
}


