// namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.AdresMergerHandler;
//
// using AssociationRegistry.Admin.Api.GrarConsumer;
// using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
// using AssociationRegistry.Admin.Api.Infrastructure.AWS;
// using AutoFixture;
// using Common.AutoFixture;
// using Grar.GrarConsumer.TeHeradresserenLocaties;
// using Moq;
// using Xunit;
//
// public class Given_Locations_Found_For_Multiple_VCodes
// {
//     [Fact]
//     public async Task Then_It_Sends_Multiple_Messages_On_The_Queue()
//     {
//         var fixture = new Fixture().CustomizeAdminApi();
//         var adresId = fixture.Create<int>();
//
//         var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
//         var teHeradresserenLocatiesFinder = new Mock<ITeHeradresserenLocatiesFinder>();
//         var teHeradresserenLocatiesMessages = fixture.CreateMany<TeHeradresserenLocatiesMessage>();
//
//         teHeradresserenLocatiesFinder.Setup(s => s.Find(adresId))
//                                      .ReturnsAsync(teHeradresserenLocatiesMessages.ToArray);
//
//         var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, teHeradresserenLocatiesFinder.Object);
//
//         await sut.Handle(adresId);
//
//         sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(
//                                         It.IsAny<TeHeradresserenLocatiesMessage>()),
//                                     Times.Exactly(teHeradresserenLocatiesMessages.Count()));
//     }
// }
