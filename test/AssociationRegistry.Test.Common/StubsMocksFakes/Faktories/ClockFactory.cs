namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;

public class ClockFactory
{
    public ClockStub Stub(DateOnly date)
        => new(date);
}
