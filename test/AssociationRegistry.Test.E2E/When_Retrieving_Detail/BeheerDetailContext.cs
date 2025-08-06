namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests.VZER;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(BeheerDetailCollection))]
public class BeheerDetailCollection : ICollectionFixture<BeheerDetailContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class BeheerDetailContext : TestContextBase<EmptyScenario, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public BeheerDetailContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
        CommandResult = await new BeheerdetailRequestFactory().ExecuteRequest(ApiSetup);
    }
}
