namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public record Datum
{
    public const string Format = "yyyy-MM-dd";

    private Datum(DateOnly value)
    {
        Value = value;
    }

    public DateOnly Value { get; }

    public static Datum Create(DateOnly startdatum)
        => new(startdatum);

    public static Datum? CreateOptional(DateOnly? startdatum)
        => startdatum.HasValue ? new Datum(startdatum.Value) : null;

    public static Datum? Hydrate(DateOnly? dateOnly)
        => dateOnly.HasValue ? new Datum(dateOnly.Value) : null;

    public static bool Equals(Datum? oldStartdatum, Datum? newStartdatum)
    {
        if (oldStartdatum is null && newStartdatum is null)
            return true;

        return oldStartdatum is not null && newStartdatum is not null && newStartdatum.Equals(oldStartdatum);
    }

    public virtual bool Equals(Datum? other)
    {
        if (ReferenceEquals(objA: null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Value == other.Value;
    }

    public static implicit operator DateOnly?(Datum datum)
        => datum.Value;

    public bool IsInFutureOf(DateOnly datum)
        => Value > datum;

    public bool IsInFutureOf(Datum datum)
        => Value > datum;

    public bool IsInPastOf(Datum? datum)
        => datum is not null && Value < datum;

    public static bool CanParse(string dateOnly)
        => DateOnly.TryParseExact(dateOnly, Format, out _);
}
