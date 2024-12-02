namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;

using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.GrarUpdates.TeOnkoppelenLocaties;
using Moq;

public static class SetupMockExtension
{
    public static void CaptureQueueReaddressMessage(
        this Mock<ISqsClientWrapper> sqsClientWrapper,
        Action<TeHeradresserenLocatiesMessage> action)
    {
        sqsClientWrapper.Setup(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()))
                        .Callback<TeHeradresserenLocatiesMessage>(action);
    }
    public static void CaptureQueueOntkoppelMessage(
        this Mock<ISqsClientWrapper> sqsClientWrapper,
        Action<TeOntkoppelenLocatiesMessage> action)
    {
        sqsClientWrapper.Setup(v => v.QueueMessage(It.IsAny<TeOntkoppelenLocatiesMessage>()))
                        .Callback<TeOntkoppelenLocatiesMessage>(action);
    }
}
