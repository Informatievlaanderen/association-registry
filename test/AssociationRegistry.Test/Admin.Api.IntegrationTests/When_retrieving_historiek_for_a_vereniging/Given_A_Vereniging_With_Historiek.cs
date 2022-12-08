// namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_retrieving_historiek_for_a_vereniging;
//
// using AssociationRegistry.Framework;
// using Fixtures;
// using Vereniging;
// using FluentAssertions;
// using NodaTime.Extensions;
// using Xunit;
//
// public class Given_A_Vereniging_With_Historiek_Fixture : AdminApiFixture
// {
//     public const string VCode = "v0001001";
//
//     public Given_A_Vereniging_With_Historiek_Fixture() : base(nameof(Given_A_Vereniging_With_Historiek_Fixture))
//     {
//     }
//
//     public override async Task InitializeAsync()
//     {
//         await base.InitializeAsync();
//         await AddEvent(
//             VCode,
//             new VerenigingWerdGeregistreerd(
//                 VCode: VCode,
//                 Naam: "Feestcommittee Oudenaarde",
//                 KorteNaam: "FOud",
//                 KorteBeschrijving: "Het feestcommittee van Oudenaarde",
//                 Startdatum: DateOnly.FromDateTime(new DateTime(2022, 11, 9)),
//                 KboNummer: "0123456789",
//                 Status: "Actief",
//                 DatumLaatsteAanpassing: DateOnly.FromDateTime(DateTime.Today)),
//             new CommandMetadata(
//                 Initiator: "Een initiator",
//                 Tijdstip: new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
//     }
// }
//
// public class Given_A_Vereniging_With_Historiek : IClassFixture<Given_A_Vereniging_With_Historiek_Fixture>
// {
//     private const string VCode = Given_A_Vereniging_With_Historiek_Fixture.VCode;
//     private readonly HttpClient _httpClient;
//     private readonly string _goldenMasterFile;
//
//     public Given_A_Vereniging_With_Historiek(Given_A_Vereniging_With_Historiek_Fixture fixture)
//     {
//         _httpClient = fixture.HttpClient;
//         _goldenMasterFile = $"{nameof(Given_A_Vereniging_With_Historiek)}_{nameof(Then_we_get_a_historiek_response)}";
//     }
//
//     [Fact]
//     public async Task Then_we_get_a_successful_response()
//     {
//         var response = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}/historiek");
//         response.Should().BeSuccessful();
//     }
//
//     [Fact]
//     public async Task Then_we_get_a_historiek_response()
//     {
//         var responseMessage = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}/historiek");
//
//         var content = await responseMessage.Content.ReadAsStringAsync();
//
//         var goldenMaster = GetType().GetAssociatedResourceJson(_goldenMasterFile);
//
//         content.Should().BeEquivalentJson(goldenMaster);
//     }
// }
