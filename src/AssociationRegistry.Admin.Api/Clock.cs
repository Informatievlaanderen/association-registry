namespace AssociationRegistry.Admin.Api;

using System;

public class Clock : IClock
{
    public DateOnly Today
        => DateOnly.FromDateTime(DateTime.Today);
}
