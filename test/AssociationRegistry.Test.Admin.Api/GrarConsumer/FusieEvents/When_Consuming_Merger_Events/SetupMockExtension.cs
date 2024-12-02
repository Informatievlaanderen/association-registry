namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;

using Acties.HeradresseerLocaties;
using Acties.OntkoppelAdres;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Framework;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Moq;

public static class SetupMockExtension
{
    public static void CaptureQueueReaddressMessage(
        this Mock<ISqsClientWrapper> sqsClientWrapper,
        Action<HeradresseerLocatiesMessage> action)
    {
        sqsClientWrapper.Setup(v => v.QueueReaddressMessage(It.IsAny<HeradresseerLocatiesMessage>()))
                        .Callback<HeradresseerLocatiesMessage>(action);
    }
    public static void CaptureQueueOntkoppelMessage(
        this Mock<ISqsClientWrapper> sqsClientWrapper,
        Action<TeOntkoppelenLocatiesMessage> action)
    {
        sqsClientWrapper.Setup(v => v.QueueMessage(It.IsAny<TeOntkoppelenLocatiesMessage>()))
                        .Callback<TeOntkoppelenLocatiesMessage>(action);
    }
}
