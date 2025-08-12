namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using CommandHandling.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using CommandHandling.Grar.GrarUpdates.LocatieFinder;
using Common.AutoFixture;
using Moq;
using Wolverine;
using Xunit;

public class Given_No_Locations_Found
{
    [Fact]
    public async ValueTask Then_No_Messages_Are_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();
        var idempotencyKey = fixture.Create<string>();

        var messageBusMock = new Mock<IMessageBus>();
        var locatiesFinder = new Mock<ILocatieFinder>();

        locatiesFinder.Setup(s => s.FindLocaties(sourceAdresId))
                                     .ReturnsAsync(LocatiesPerVCodeCollection.Empty);

        var sut = new TeHeradresserenLocatiesProcessor(messageBusMock.Object, locatiesFinder.Object);
        await sut.Process(sourceAdresId, destinationAdresId, idempotencyKey);

        messageBusMock.Verify(v => v.SendAsync<HeradresseerLocatiesMessage>(
                                  It.IsAny<HeradresseerLocatiesMessage>(),
                                  It.IsAny<DeliveryOptions>()),
                              Times.Never());
    }
}
