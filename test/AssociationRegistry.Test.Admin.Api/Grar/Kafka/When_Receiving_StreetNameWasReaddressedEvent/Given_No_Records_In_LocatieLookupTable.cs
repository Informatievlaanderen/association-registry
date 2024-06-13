// namespace AssociationRegistry.Test.Admin.Api.Grar.Kafka.When_Receiving_StreetNameWasReaddressedEvent;
//
// using AssociationRegistry.Admin.Api.GrarSync;
// using AssociationRegistry.Admin.Schema.Detail;
// using Xunit;
// using FluentAssertions;
//
// public class Given_No_Records_In_LocatieLookupTable
// {
//     public Given_No_Records_In_LocatieLookupTable()
//     {
//
//     }
//
//     [Fact]
//     public async Task Then_No_Messages_Are_Queued()
//     {
//         var locatieFinder = new LocatieFinder(new List<LocatieLookupDocument>());
//
//         var sut = new TeHeradresserenLocatiesMapper(locatieFinder);
//         var result = await sut.ForAddress("1","2", "idempotencyKey");
//
//         result.Should().BeEmpty();
//     }
// }
