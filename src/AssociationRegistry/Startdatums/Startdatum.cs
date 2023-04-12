namespace AssociationRegistry.Startdatums;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;
using Primitives;

public class Startdatum : ValueObject<Startdatum>
{
    private Startdatum(DateOnly datum)
    {
        Value = datum;
    }

    public DateOnly Value { get; }

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

    public static Startdatum? Create(IClock clock, NullOrEmpty<DateOnly> commandStartdatum)
    {
        if (!commandStartdatum.HasValue) return null;

        return Create(commandStartdatum.Value, datum => Validate(clock.Today, datum));
    }

    public static Startdatum? Create(IClock clock, DateOnly commandStartdatum)
    {
        return Create(commandStartdatum, datum => Validate(clock.Today, datum));
    }

    public static bool Equals(Startdatum? oldStartdatum, Startdatum? newStartdatum)
    {
        if (oldStartdatum is null && newStartdatum is null)
            return true;

        return oldStartdatum is not null && newStartdatum is not null && newStartdatum.Equals(oldStartdatum);
    }
}
