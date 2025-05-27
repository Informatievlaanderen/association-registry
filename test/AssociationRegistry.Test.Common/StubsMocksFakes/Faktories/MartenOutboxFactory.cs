namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using global::AutoFixture;
using Moq;
using Wolverine.Marten;

public class MartenOutboxFactory
{
    public Mock<IMartenOutbox> Mock()
        => new();

    public MartenOutboxFactory(Fixture fixture)
    {
    }
}