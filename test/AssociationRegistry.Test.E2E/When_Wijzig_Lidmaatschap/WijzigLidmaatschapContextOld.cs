namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Schema;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;
using LidmaatschapWerdToegevoegdScenario = Scenarios.Givens.FeitelijkeVereniging.LidmaatschapWerdToegevoegdScenario;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(WijzigLidmaatschapCollection))]
public class WijzigLidmaatschapCollection : ICollectionFixture<WijzigLidmaatschapContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class WijzigLidmaatschapContext : TestContextBase<LidmaatschapWerdToegevoegdScenario, WijzigLidmaatschapRequest>
{
    protected override LidmaatschapWerdToegevoegdScenario InitializeScenario()
        => new(new MultipleWerdGeregistreerdScenario());

    public WijzigLidmaatschapContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    public override async ValueTask ExecuteCommandRequests(IScenario scenario)
    {
        CommandResult = await new WijzigLidmaatschapRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

// CollectionFixture for database setup ==> Context
public class DatabaseFixture2 : IDisposable
{

    // Fullblown is assembly fixture
    public DatabaseFixture2(FullBlownApiSetup setup)
    {

        // ... initialize data in the test database ...
    }

    public void Dispose()
    {
        // ... clean up test data from the database ...
    }
}



[CollectionDefinition("Database collection 2")]
public class DbCollection2 : ICollectionFixture<DatabaseFixture2>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection(nameof(WijzigLidmaatschapCollection))]
public class DatabaseTestClass1
{
    WijzigLidmaatschapContext fixture;

    public DatabaseTestClass1(WijzigLidmaatschapContext fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        var response = fixture.ApiSetup.PublicApiHost.GetPubliekDetail(fixture.VCode);
        response.Should().NotBeNull();
    }


    [Fact]
    public void Test2()
    {
        var response = fixture.ApiSetup.PublicApiHost.GetPubliekDetail(fixture.VCode);
        response.Should().NotBeNull();
    }
}

[Collection(nameof(WijzigLidmaatschapCollection))]
public class DatabaseTestClass2
{
    private readonly WijzigLidmaatschapContext fixture;

    public DatabaseTestClass2(WijzigLidmaatschapContext fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        var response = fixture.ApiSetup.PublicApiHost.GetBeheerDetail(fixture.VCode);
        response.Should().NotBeNull();
        // ...
    }

}

[Collection("Database collection 2")]
public class DatabaseTestClass121
{
    DatabaseFixture2 fixture;

    public DatabaseTestClass121(DatabaseFixture2 fixture)
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

[Collection("Database collection 2")]
public class DatabaseTestClass22
{
    [Fact]
    public void Test1()
    {
        1.Should().Be(1); // Use fixture to access the initialized data
        // ...
    }

}
