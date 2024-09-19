namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Admin.Schema;
using Alba;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Marten.Events;
using Scenarios;
using System.Net;
using Xunit;

[CollectionDefinition(nameof(StopVerenigingContext<AdminApiSetup>))]
public class StopVerenigingCollection : ICollectionFixture<StopVerenigingContext<AdminApiSetup>>
{
}

[CollectionDefinition(nameof(PubliekStopVerenigingCollection))]
public class PubliekStopVerenigingCollection : ICollectionFixture<StopVerenigingContext<PublicApiSetup>>
{
}

public class StopVerenigingContext<T> : End2EndContext<StopVerenigingRequest, FeitelijkeVerenigingWerdGeregistreerdScenario>, IAsyncLifetime
    where T : IApiSetup, new()
{
    protected override string SchemaName => $"stopvereniging{GetType().GetGenericArguments().First().Name}";
    public override FeitelijkeVerenigingWerdGeregistreerdScenario Scenario => new();

    public override StopVerenigingRequest Request => new()
    {
        Einddatum = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)
    };

    public StopVerenigingContext() : base(new T())
    {
        VCode = Scenario.VCode;
    }

    public async Task InitializeAsync()
    {
        await AdminApiHost.DocumentStore().Advanced.ResetAllData();

        await Given(Scenario);

        await AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/{Scenario.VCode}/stop");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public Metadata Metadata { get; set; }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
