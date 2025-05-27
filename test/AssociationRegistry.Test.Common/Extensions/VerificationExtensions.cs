namespace AssociationRegistry.Test.Common.Extensions;

using AssociationRegistry.Framework;
using Moq;
using Wolverine;
using Wolverine.Marten;

public static class VerificiationExtensions
{
    public static void VerifyCommandInvoked<TCommand>(this Mock<IMessageBus> source, TCommand commandComparison, Times times, string? initiator = null, long? expectedVersion = null)
        where TCommand : class
    {
        source.Verify(x =>
                          x.InvokeAsync(
                              It.Is<CommandEnvelope<TCommand>>(x =>
                                                                   x.Command == commandComparison &&
                                                                   (initiator == null || x.Metadata.Initiator == initiator ) &&
                                                                   (expectedVersion == null || x.Metadata.ExpectedVersion == expectedVersion)),
                              It.IsAny<CancellationToken>(),
                              It.IsAny<TimeSpan?>()),
                      times);
    }

    public static void VerifyCommandInvoked<TCommand>(this Mock<IMessageBus> source, Func<CommandEnvelope<TCommand>, bool> commandComparison, Times times)
        where TCommand : class
    {
        source.Verify(x =>
                          x.InvokeAsync(
                              It.Is<CommandEnvelope<TCommand>>(x => commandComparison(x)),
                              It.IsAny<CancellationToken>(),
                              It.IsAny<TimeSpan?>()),
                      times);
    }

    public static void VerifyCommandPublished<TCommand>(this Mock<IMartenOutbox> source, Func<CommandEnvelope<TCommand>, bool> commandComparison, Times times)
        where TCommand : class
    {
        source.Verify(x =>
                          x.PublishAsync(
                              It.Is<CommandEnvelope<TCommand>>(x => commandComparison(x)),
                              It.IsAny<DeliveryOptions?>()),
                      times);
    }

    public static void VerifyCommandSent<TCommand>(this Mock<IMartenOutbox> source, Func<CommandEnvelope<TCommand>, bool> commandComparison, Times times)
        where TCommand : class
    {
        source.Verify(x =>
                          x.SendAsync(
                              It.Is<CommandEnvelope<TCommand>>(x => commandComparison(x)),
                              It.IsAny<DeliveryOptions?>()),
                      times);
    }
}
