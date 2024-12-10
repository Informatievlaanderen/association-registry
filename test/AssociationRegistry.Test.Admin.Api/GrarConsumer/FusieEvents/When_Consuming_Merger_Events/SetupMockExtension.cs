namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;

using Acties.GrarConsumer;
using Acties.GrarConsumer.HeradresseerLocaties;
using Acties.GrarConsumer.OntkoppelAdres;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Framework;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Moq;
using Wolverine;
using Wolverine.Marten;

public static class SetupMockExtension
{
    public static void CaptureQueueOverkoepelendeGrarMessage(
        this Mock<ISqsClientWrapper> sqsClientWrapper,
        Action<OverkoepelendeGrarConsumerMessage> action)
    {
        sqsClientWrapper.Setup(v => v.QueueMessage(It.IsAny<OverkoepelendeGrarConsumerMessage>()))
                        .Callback(action);
    }


    public static void CaptureOutboxSendAsyncMessage<TMessage>(
        this Mock<IMartenOutbox> outbox,
        Action<TMessage> action)
    {
        outbox.Setup(v => v.SendAsync(It.IsAny<TMessage>(), It.IsAny<DeliveryOptions>()))
              .Callback<TMessage, DeliveryOptions>((arg1, _) => action(arg1));
    }
}
