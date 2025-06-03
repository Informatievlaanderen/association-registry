namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using global::AutoFixture;
using Moq;
using Wolverine;

public class MessageBusFactory
{
    public Mock<IMessageBus> Mock()
        => new();

    public MessageBusFactory(IFixture fixture)
    {
    }
}
