namespace AssociationRegistry.Startdatums;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class Startdatum : ValueObject<Startdatum>
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
        Throw<InvalidStartdatumFuture>.If(today < datum);
    }

    protected override IEnumerable<object> Reflect()
    {
        yield return Value;
    }
}
