// namespace AssociationRegistry.Test.Acm.Api.Tests.VerenigingenPerRijksregisternummer;
//
// using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;
// using AutoFixture;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using Xunit;
//
// public class ControllerPutTests
// {
//     private readonly Fixture _fixture;
//
//     public ControllerPutTests()
//     {
//         _fixture = new Fixture();
//     }
//
//     [Fact]
//     public async Task Tests()
//     {
//         var controllerGetTests = new ControllerGetTests();
//
//         controllerGetTests.Test_7103();
//         controllerGetTests.Test_9803();
//         
//         Test_new_rijksregisternummer();
//
//         Test_existing_rijksregisternummer();
//     }
//
//     private async Task Test_new_rijksregisternummer()
//     {
//         var controller = new VerenigingenPerRijksregisternummerController();
//
//         var rijksregisternummer = _fixture.Create<string>();
//         var verenigingen = new List<PutVerenigingenRequest.Vereniging>
//         {
//             new()
//             {
//                 Id = _fixture.Create<string>(),
//                 Naam = _fixture.Create<string>(),
//             },
//             new()
//             {
//                 Id = _fixture.Create<string>(),
//                 Naam = _fixture.Create<string>(),
//             },
//         };
//         
//         controller.Put(rijksregisternummer, new PutVerenigingenRequest
//         {
//             Verenigingen = verenigingen
//         });
//
//         var response = (OkObjectResult) await controller.Get(rijksregisternummer);
//         
//         var verenigingenResponse = (GetVerenigingenResponse)response.Value!;
//
//         verenigingenResponse.Rijksregisternummer.Should().Be(rijksregisternummer);
//         verenigingenResponse.Verenigingen.ToList().Should().BeEquivalentTo(verenigingen);
//     }
//
//     private async Task Test_existing_rijksregisternummer()
//     {
//         var controller = new VerenigingenPerRijksregisternummerController();
//
//         var rijksregisternummer = "71035463546";
//         var verenigingen = new List<PutVerenigingenRequest.Vereniging>
//         {
//             new()
//             {
//                 Id = _fixture.Create<string>(),
//                 Naam = _fixture.Create<string>(),
//             },
//             new()
//             {
//                 Id = _fixture.Create<string>(),
//                 Naam = _fixture.Create<string>(),
//             },
//         };
//         
//         controller.Put(rijksregisternummer, new PutVerenigingenRequest
//         {
//             Verenigingen = verenigingen
//         });
//
//         var response = (OkObjectResult) await controller.Get(rijksregisternummer);
//         
//         var verenigingenResponse = (GetVerenigingenResponse)response.Value!;
//
//         verenigingenResponse.Rijksregisternummer.Should().Be(rijksregisternummer);
//         verenigingenResponse.Verenigingen.ToList().Should().BeEquivalentTo(verenigingen);
//     }
// }
