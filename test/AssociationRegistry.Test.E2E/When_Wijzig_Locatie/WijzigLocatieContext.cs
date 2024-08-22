﻿namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Admin.Schema;
using Alba;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Marten.Events;
using Scenarios;
using System.Net;
using Vereniging;
using Xunit;
using Adres = Admin.Api.Verenigingen.Common.Adres;

[CollectionDefinition(nameof(WijzigLocatieContext<AdminApiSetup>))]
public class WijzigLocatieCollection : ICollectionFixture<WijzigLocatieContext<AdminApiSetup>>
{
}

public class WijzigLocatieContext<T> : End2EndContext<WijzigLocatieRequest, FeitelijkeVerenigingWerdGeregistreerdScenario>, IAsyncLifetime
    where T : IApiSetup, new()
{
    protected override string SchemaName => $"wijzig{GetType().GetGenericArguments().First().Name}";
    public override FeitelijkeVerenigingWerdGeregistreerdScenario Scenario => new();

    public override WijzigLocatieRequest Request => new()
    {
        Locatie =
            new TeWijzigenLocatie
            {
                Naam = "Kantoor",
                Adres = new Adres
                {
                    Straatnaam = "Leopold II-laan",
                    Huisnummer = "99",
                    Busnummer = "",
                    Postcode = "9200",
                    Gemeente = "Dendermonde",
                    Land = "België",
                },
                IsPrimair = true,
                Locatietype = Locatietype.Correspondentie,
            },
    };

    public WijzigLocatieContext() : base(new T())
    {
        VCode = Scenario.VCode;
    }

    public async Task InitializeAsync()
    {
        await AdminApiHost.DocumentStore().Advanced.ResetAllData();

        await Given(Scenario);

        await AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/{Scenario.VCode}/locaties/{Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await WaitForAdresMatchEvent();

        await ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public Metadata Metadata { get; set; }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
