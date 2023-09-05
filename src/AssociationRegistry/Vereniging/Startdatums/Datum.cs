namespace AssociationRegistry.Vereniging;

public record Datum
{
    public static readonly Datum Leeg = new((DateOnly?)null);

    private Datum(DateOnly? value)
    {
        Value = value;
    }

    public bool IsLeeg
        => Equals(Leeg);

    public DateOnly? Value { get; }

    public static Datum Create(DateOnly? startdatum)
        => new(startdatum);

    public static Datum Hydrate(DateOnly? dateOnly)
        => new(dateOnly);

    public static bool Equals(Datum? oldStartdatum, Datum? newStartdatum)
    {
        if (oldStartdatum is null && newStartdatum is null)
            return true;

        return oldStartdatum is not null && newStartdatum is not null && newStartdatum.Equals(oldStartdatum);
    }

    public static implicit operator DateOnly?(Datum datum)
        => datum.Value;

    public bool IsInFutureOf(DateOnly datum)
        => Value > datum;

    public bool IsInFutureOf(Datum datum)
        => Value > datum;

    public bool IsInPastOf(Datum? datum)
        => datum is not null && Value < datum;
}
