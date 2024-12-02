namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Consuming_Merger_Events;

using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
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
}
