namespace AssociationRegistry.Test.E2E.When_Verwijder_Validatie_Bankrekeningnummer;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class MaakValidatieBankrekeningnummerOngedaanContext : TestContextBase<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigdScenario, NullRequest>
{

    protected override AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigdScenario InitializeScenario()
        => new();

    public MaakValidatieBankrekeningnummerOngedaanContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigdScenario scenario)
    {
        CommandResult = await new MaakValidatieBankrekeningnummerOngedaanRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(MaakValidatieBankrekeningnummerOngedaanCollection))]
public class MaakValidatieBankrekeningnummerOngedaanCollection : ICollectionFixture<MaakValidatieBankrekeningnummerOngedaanContext>
{
}
