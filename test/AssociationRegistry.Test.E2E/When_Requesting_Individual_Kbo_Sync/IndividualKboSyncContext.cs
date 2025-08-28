// namespace AssociationRegistry.Test.E2E.When_Requesting_Individual_Kbo_Sync;
//
// using Events;
// using Framework.AlbaHost;
// using Framework.ApiSetup;
// using Framework.TestClasses;
// using Scenarios.Givens.MetRechtspersoonlijkheid;
// using Scenarios.Requests;
// using Scenarios.Requests.SuperAdmin;
// using Xunit;
//
// [CollectionDefinition(nameof(IndividualKboSyncCollection))]
// public class IndividualKboSyncCollection : ICollectionFixture<IndividualKboSyncContext>
// {
// }
//
// public class IndividualKboSyncContext : TestContextBase<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario, NullRequest>
// {
//     public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd RegistratieData
//         => Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
//
//     protected override VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
//         => new();
//
//     public IndividualKboSyncContext(FullBlownApiSetup apiSetup) : base(apiSetup)
//     {
//     }
//
//     protected override async ValueTask ExecuteScenario(VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
//     {
//         CommandResult = await new IndividualKboSyncRequestFactory(
//             SmartHttpClient.Create(ApiSetup.AdminApiHost, ApiSetup.SuperAdminHttpClient),
//                                                                   Scenario).ExecuteRequest(ApiSetup);
//     }
// }
