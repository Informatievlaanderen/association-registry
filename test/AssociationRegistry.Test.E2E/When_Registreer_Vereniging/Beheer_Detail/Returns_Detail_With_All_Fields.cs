// namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Detail;
//
// using Alba;
// using FluentAssertions;
// using Xunit;
//
// [Collection(nameof(RegistreerVerenigingContext))]
// public class Returns_Detail_With_All_Field: RegistreerVerenigingContext
// {
//     private readonly IAlbaHost theHost;
//
//     public Returns_Detail_With_All_Field(AppFixture fixture) : base(fixture)
//     {
//         theHost = fixture.Host;
//     }
//
//     [Fact]
//     public async Task JsonContentMatches()
//     {
//         var result = await theHost.GetAsText($"/v1/verenigingen/{ResultingVCode}");
//
//         var expected = new DetailVerenigingResponseTemplate()
//                       .FromEvent(Scenario.FeitelijkeVerenigingWerdGeregistreerd)
//                       .WithDatumLaatsteAanpassing(Scenario.Metadata.Tijdstip);
//
//         result.Should().BeEquivalentJson(expected);
//     }
// }
