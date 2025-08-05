namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeOntkoppelenHandlerTests;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
using AutoFixture;
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

        var messageBusMock = new Mock<IMessageBus>();
        var locatiesFinder = new Mock<ILocatieFinder>();

        locatiesFinder.Setup(s => s.FindLocaties(sourceAdresId))
                      .ReturnsAsync(LocatiesPerVCodeCollection.Empty);

        var sut = new TeOntkoppelenLocatiesProcessor(messageBusMock.Object, locatiesFinder.Object);
        await sut.Process(sourceAdresId);

        messageBusMock.Verify(v => v.SendAsync(It.IsAny<OntkoppelLocatiesMessage>(), It.IsAny<DeliveryOptions>()), Times.Never());
    }
}
