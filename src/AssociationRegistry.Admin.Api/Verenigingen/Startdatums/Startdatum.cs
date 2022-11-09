namespace AssociationRegistry.Admin.Api.Verenigingen.Startdatums;

using System;
using Exceptions;

public class Startdatum
{
    private Startdatum(DateOnly datum)
    {
        Value = datum;
    }

    public DateOnly Value { get; }

    public static Startdatum? Create(IClock clock, DateOnly? maybeDatum)
        => Create(maybeDatum, datum => Validate(clock.Today, datum));

    internal static Startdatum? Create(DateOnly? maybeDatum, Action<DateOnly>? validate = null)
    {
        if (maybeDatum is not { } datum) return null;
        validate?.Invoke(datum);
        return new Startdatum(datum);
    }

    private static void Validate(DateOnly today, DateOnly datum)
    {
        Throw<InvalidStartdatum>.If(today < datum);
    }
}
