// namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Vzer;
//
// using Scenario.Vertegenwoordigers.Vzer;
//
// [Collection(nameof(ProjectionContext))]
// public class Given_VertegenwoordigerWerdGewijzigdMetPersoonsgegevens(
//     BeheerDetailScenarioFixture<VertegenwoordigerWerdGewijzigdMetPersoonsgegevensScenario> fixture)
//     : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdGewijzigdMetPersoonsgegevensScenario>
// {
//     [Fact]
//     public void Metadata_Is_Updated()
//         => fixture.Result
//                   .Metadata.Version.Should().Be(2);
//
//     [Fact]
//     public void Vertegenwoordiger_Is_Toegevoegd()
//     {
//         var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdGewijzigd.VertegenwoordigerId);
//
//         vertegenwoordiger.Should().BeEquivalentTo(new
//         {
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.VertegenwoordigerPersoonsgegevens.Roepnaam,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.Rol,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.IsPrimair,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.Email,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.Telefoon,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.Mobiel,
//             fixture.Scenario.VertegenwoordigerWerdGewijzigd.SocialMedia,
//             VertegenwoordigerContactgegevens = new
//             {
//                 fixture.Scenario.VertegenwoordigerWerdGewijzigd.IsPrimair,
//                 fixture.Scenario.VertegenwoordigerWerdGewijzigd.Email,
//                 fixture.Scenario.VertegenwoordigerWerdGewijzigd.Telefoon,
//                 fixture.Scenario.VertegenwoordigerWerdGewijzigd.Mobiel,
//                 fixture.Scenario.VertegenwoordigerWerdGewijzigd.SocialMedia
//             },
//         });
//     }
// }
