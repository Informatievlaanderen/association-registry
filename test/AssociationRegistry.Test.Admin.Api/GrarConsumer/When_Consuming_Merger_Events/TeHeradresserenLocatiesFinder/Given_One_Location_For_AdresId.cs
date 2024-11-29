// namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.TeHeradresserenLocatiesFinder;
//
// using AssociationRegistry.Admin.Api.GrarConsumer;
// using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
// using AssociationRegistry.Admin.Schema.Detail;
// using AutoFixture;
// using Common.AutoFixture;
// using FluentAssertions;
// using Grar.GrarConsumer.TeHeradresserenLocaties;
// using Grar.Models;
// using Moq;
// using Xunit;
//
// public class Given_One_Location_For_AdresId
// {
//     [Fact]
//     public async Task Then_It_Returns_Grouped_Messages()
//     {
//         var fixture = new Fixture().CustomizeAdminApi();
//         var locatieFinder = new Mock<ILocatieFinder>();
//         var adresId = fixture.Create<int>();
//         var locatieLookupDocument = fixture.Create<LocatieLookupData>();
//
//         locatieFinder.Setup(s => s.FindLocaties(adresId))
//                      .ReturnsAsync([locatieLookupDocument]);
//
//         var sut = new TeHeradresserenLocatiesFinder(locatieFinder.Object);
//
//         var actual = await sut.Find(adresId);
//
//         actual.Should().NotBeEmpty();
//         actual.Count().Should().Be(1);
//
//         actual.First().Should().BeEquivalentTo(
//             new TeHeradresserenLocatiesMessage(
//                 locatieLookupDocument.VCode, new List<TeHeradresserenLocatie>
//                 {
//                     new TeHeradresserenLocatie(locatieLookupDocument.LocatieId, locatieLookupDocument.AdresId)
//                 },
//                 ""), config: options => options.Excluding(x => x.idempotencyKey));
//     }
// }
