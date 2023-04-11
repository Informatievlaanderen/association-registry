namespace AssociationRegistry.Startdatums;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;
using Primitives;

public class Startdatum : ValueObject<Startdatum>
{
    public static Startdatum? NietOpgegeven = Startdatum.Create(null);

    private Startdatum(DateOnly datum)
    {
        Value = datum;
    }

    public DateOnly? Value { get; }

    public bool HasValue
        => Value != null;

    internal static Startdatum? Create(DateOnly? maybeDatum)
    {
        if (maybeDatum is not { } datum) return null;
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

    public static Startdatum? Create(NullOrEmpty<DateOnly> commandStartdatum)
    {
        if (!commandStartdatum.HasValue) return null;

        return FromDateOnly(commandStartdatum.Value);
    }

    public static Startdatum? FromDateOnly(DateOnly commandStartdatum)
    {
        return Create(commandStartdatum);
    }
}
