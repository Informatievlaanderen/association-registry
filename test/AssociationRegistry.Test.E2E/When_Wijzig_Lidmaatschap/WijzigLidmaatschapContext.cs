namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;
using Xunit;

[CollectionDefinition("WijzigLidmaatschapContext")]
public class DatabaseCollection : ICollectionFixture<WijzigLidmaatschapContext>
{
}

public class WijzigLidmaatschapContext: TestContextBase<WijzigLidmaatschapRequest>, IDisposable
{
    public const string Name = "WijzigLidmaatschapContext";
    public VCode VCode => RequestResult.VCode;
    public LidmaatschapWerdToegevoegdScenario Scenario { get; }

    public WijzigLidmaatschapContext()
    {
        // ApiSetup = apiSetup;
        // Scenario = new(new MultipleWerdGeregistreerdScenario());
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigLidmaatschapRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // TODO release managed resources here
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture(FullBlownApiSetup setup)
    {

        // ... initialize data in the test database ...
    }

    public void Dispose()
    {
        // ... clean up test data from the database ...
    }

}

[CollectionDefinition("Database collection")]
public class DbCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection("Database collection")]
public class DatabaseTestClass1
{
    DatabaseFixture fixture;

    public DatabaseTestClass1(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        1.Should().Be(1); // Use fixture to access the initialized data
        // ...
    }
}

[Collection("Database collection")]
public class DatabaseTestClass2
{
    [Fact]
    public void Test1()
    {
        1.Should().Be(1); // Use fixture to access the initialized data
        // ...
    }}
