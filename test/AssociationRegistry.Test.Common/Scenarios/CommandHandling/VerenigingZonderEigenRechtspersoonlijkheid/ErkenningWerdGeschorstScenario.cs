// namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
//
// using global::AutoFixture;
// using AutoFixture;
// using DecentraalBeheer.Vereniging;
// using Events;
//
// public class ErkenningWerdGeschorstScenario : CommandhandlerScenarioBase
// {
//     public override VCode VCode => VCode.Create("V0009002");
//
//     public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
//         VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
//
//     public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
//     public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;
//
//     public ErkenningWerdGeschorstScenario()
//     {
//         var fixture = new Fixture().CustomizeAdminApi();
//
//         VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
//             fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
//             {
//                 VCode = VCode,
//             };
//
//         ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>();
//
//         ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>()
//             with
//             {
//                 ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
//             };
//     }
//
//     public override IEnumerable<IEvent> Events() =>
//         new IEvent[]
//         {
//             VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd,
//             ErkenningWerdGeschorst,
//         };
// }
