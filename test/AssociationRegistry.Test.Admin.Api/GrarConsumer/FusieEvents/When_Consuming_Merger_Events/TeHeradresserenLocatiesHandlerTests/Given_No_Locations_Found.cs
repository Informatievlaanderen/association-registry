namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
using AutoFixture;
using Common.AutoFixture;
using Moq;
using Xunit;

public class Given_No_Locations_Found
{
    [Fact]
    public async Task Then_No_Messages_Are_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();
        var idempotencyKey = fixture.Create<string>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var locatiesFinder = new Mock<ILocatieFinder>();

        locatiesFinder.Setup(s => s.FindLocaties(sourceAdresId))
                                     .ReturnsAsync(LocatiesPerVCodeCollection.Empty);

        var sut = new TeHeradresserenLocatiesProcessor(sqsClientWrapperMock.Object, locatiesFinder.Object);
        await sut.Process(sourceAdresId, destinationAdresId, idempotencyKey);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<HeradresseerLocatiesMessage>()), Times.Never());
    }
}
