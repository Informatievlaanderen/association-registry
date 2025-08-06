namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests.VZER;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection : ICollectionFixture<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext : TestContextBase<EmptyScenario, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
        CommandResult = await new RegistreerVZERRequestFactory().ExecuteRequest(ApiSetup);
    }
}
