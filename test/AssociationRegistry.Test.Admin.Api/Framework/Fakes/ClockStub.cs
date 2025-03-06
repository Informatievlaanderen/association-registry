namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Framework;
using System;

public class ClockStub : IClock
{
    public ClockStub(DateOnly now)
    {
        Today = now;
    }

    public DateOnly Today { get; }
}
