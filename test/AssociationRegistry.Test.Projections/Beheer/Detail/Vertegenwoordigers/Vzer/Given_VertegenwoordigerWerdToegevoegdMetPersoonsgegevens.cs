// namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Vzer;
//
// using Admin.ProjectionHost.Projections.Detail;
// using Admin.Schema.Detail;
// using Contracts.JsonLdContext;
// using Scenario.Vertegenwoordigers.Vzer;
//
// [Collection(nameof(ProjectionContext))]
// public class Given_VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
//     BeheerDetailScenarioFixture<VertegenwoordigerWerdToegevoegdMetPersoonsgegevensScenario> fixture)
//     : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdToegevoegdMetPersoonsgegevensScenario>
// {
//     [Fact]
//     public void Metadata_Is_Updated()
//         => fixture.Result
//                   .Metadata.Version.Should().Be(2);
//
//     [Fact]
//     public void Vertegenwoordiger_Is_Toegevoegd()
//     {
//         var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerId);
//
//         vertegenwoordiger.Should().BeEquivalentTo(new Vertegenwoordiger
//         {
//             JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
//                 JsonLdType.Vertegenwoordiger, fixture.Scenario.VCode,
//                 fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerId.ToString()),
//             VertegenwoordigerId = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerId,
//             Insz = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Insz,
//             Achternaam = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Achternaam,
//             Voornaam = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Voornaam,
//             Roepnaam = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Roepnaam,
//             Rol = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Rol,
//             IsPrimair = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.IsPrimair,
//             Email = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Email,
//             Telefoon = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Telefoon,
//             Mobiel = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Mobiel,
//             SocialMedia = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.SocialMedia,
//             Bron = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.Bron,
//
//             VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
//             {
//                 JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
//                     JsonLdType.VertegenwoordigerContactgegeven, fixture.Scenario.VCode,
//                     fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerId.ToString()),
//                 IsPrimair = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.IsPrimair,
//                 Email = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Email,
//                 Telefoon = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Telefoon,
//                 Mobiel = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Mobiel,
//                 SocialMedia = fixture.Scenario.VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.SocialMedia,
//             },
//         });
//     }
// }
