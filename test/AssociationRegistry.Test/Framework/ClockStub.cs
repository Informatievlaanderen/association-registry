namespace AssociationRegistry.Test.Framework;

using AssociationRegistry.Framework;
using System;

public class ClockStub : IClock
{
    private readonly DateTime _now;

    public ClockStub(DateTime now)
    {
        _now = now;
    }

    public ClockStub(DateOnly now)
    {
        _now = now.ToDateTime(new TimeOnly());
    }

    public DateOnly Today
        => DateOnly.FromDateTime(_now);
}
