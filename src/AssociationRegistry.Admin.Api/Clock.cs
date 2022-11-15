namespace AssociationRegistry.Admin.Api;

using System;
using Framework;

public class Clock : IClock
{
    public DateOnly Today
        => DateOnly.FromDateTime(DateTime.Today);
}
