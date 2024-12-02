// namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Mapping_LocatieIdsPerVCode;
//
// using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
// using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
// using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
// using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
// using AssociationRegistry.Grar.Models;
// using AssociationRegistry.Test.Common.AutoFixture;
// using AutoFixture;
// using FluentAssertions;
// using Grar.LocatieFinder;
// using Xunit;
//
// public class Given_Locaties_For_Different_VCodes
// {
//     [Fact]
//     public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
//     {
//         var fixture = new Fixture().CustomizeAdminApi();
//         var destinationAdresId = fixture.Create<int>();
//
//         var locatieLookupData = new When_Grouping_LocatieLookupData.Given_Locaties_For_Different_VCodes.LocatieLookupTestData(fixture, fixture.Create<string>());
//
//         var actual = LocatiesVolgensVCodeGrouper.Group(locatieLookupData, destinationAdresId);
//
//         var sut = TeHeradresserenLocatiesMessageMapper
//
//         actual.Should().BeEquivalentTo(
//         [
//             new TeHeradresserenLocatiesMessage(
//                 locatieLookupData.VCode1,
//                 locatieLookupData.For(locatieLookupData.VCode1)
//                                  .Select(x => new TeHeradresserenLocatie(x.LocatieId, destinationAdresId.ToString()))
//                                  .ToList(),
//                 ""),
//             new TeHeradresserenLocatiesMessage(
//                 locatieLookupData.VCode2,
//                 locatieLookupData.For(locatieLookupData.VCode2)
//                                  .Select(x => new TeHeradresserenLocatie(x.LocatieId, destinationAdresId.ToString()))
//                                  .ToList(),
//                 ""),
//         ]);
//     }
//
//     public class LocatieIdsPerVCodeTestData : Dictionary<string, int[]>
//     {
//         public string VCode1 => "1";
//         public string VCode2 => "2";
//
//         public LocatieIdsPerVCodeTestData For(string vCode)
//         {
//             return new(this.Where(x => x.VCode == vCode)
//                            .ToArray());
//         }
//
//         private LocatieIdsPerVCodeTestData(IEnumerable<LocatieMetVCode> data) : base(data)
//         {
//         }
//
//         public LocatieIdsPerVCodeTestData(IFixture fixture)
//         {
//             var data = new LocatieIdsPerVCodeCollection(new Dictionary<string, int[]>()
//             {
//                 {
//                     fixture.Create<LocatieIdsPerVCodeCollection>() with
//                     {
//                         VCode = VCode1,
//                         LocatieId = 1
//                     }
//                 }
//             });
//             AddRange(
//             [
//                 fixture.Create<LocatieIdsPerVCodeCollection>() with
//                 {
//                     VCode = VCode1,
//                     LocatieId = 1
//                 },
//                 fixture.Create<LocatieIdsPerVCodeCollection>() with
//                 {
//                     VCode = VCode2,
//                     LocatieId = 1
//                 },
//                 fixture.Create<LocatieIdsPerVCodeCollection>() with
//                 {
//                     VCode = VCode1,
//                     LocatieId = 2
//                 }
//             ]);
//         }
//     }
// }
