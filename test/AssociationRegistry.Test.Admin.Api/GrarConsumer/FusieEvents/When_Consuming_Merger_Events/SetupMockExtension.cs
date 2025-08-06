namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarConsumer.Messaging;
using Moq;
using Wolverine;
using Wolverine.Marten;

public static class SetupMockExtension
{
    public static void CaptureQueueOverkoepelendeGrarMessage(
        this Mock<IMessageBus> sqsClientWrapper,
        Action<OverkoepelendeGrarConsumerMessage> action)
    {
        sqsClientWrapper.Setup(v => v.SendAsync(It.IsAny<OverkoepelendeGrarConsumerMessage>(), It.IsAny<DeliveryOptions>()))
                        .Callback<OverkoepelendeGrarConsumerMessage, DeliveryOptions>((message, _) => action(message));
    }


    public static void CaptureOutboxSendAsyncMessage<TMessage>(
        this Mock<IMartenOutbox> outbox,
        Action<TMessage> action)
    {
        outbox.Setup(v => v.SendAsync(It.IsAny<TMessage>(), It.IsAny<DeliveryOptions>()))
              .Callback<TMessage, DeliveryOptions>((arg1, _) => action(arg1));
    }
}
