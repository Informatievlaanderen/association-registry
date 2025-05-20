namespace AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;

using AssociationRegistry.Framework;

public class ClockStub : IClock
{
    public ClockStub(DateOnly now)
    {
        Today = now;
    }

    public DateOnly Today { get; }
}
