namespace AssociationRegistry.Startdatums;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class StartDatum : ValueObject<StartDatum>
{
    private StartDatum(DateOnly datum)
    {
        Value = datum;
    }

    public DateOnly Value { get; }

    public static StartDatum? Create(IClock clock, DateOnly? maybeDatum)
        => Create(maybeDatum, datum => Validate(clock.Today, datum));

    internal static StartDatum? Create(DateOnly? maybeDatum, Action<DateOnly>? validate = null)
    {
        if (maybeDatum is not { } datum) return null;
        validate?.Invoke(datum);
        return new StartDatum(datum);
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
