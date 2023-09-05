namespace AssociationRegistry.Vereniging.Einddatum;

public record Einddatum : IComparable<DateOnly?>
{
    public static readonly Einddatum Leeg = new((DateOnly?)null);

    private Einddatum(DateOnly? datum)
    {
        Datum = datum;
    }

    public bool IsLeeg
        => Equals(Leeg);

    public DateOnly? Datum { get; init; }

    public static Einddatum Create(DateOnly? startdatum)
        => new(startdatum);

    public static Einddatum Hydrate(DateOnly? dateOnly)
        => new(dateOnly);

    public static bool Equals(Einddatum? oldStartdatum, Einddatum? newStartdatum)
    {
        if (oldStartdatum is null && newStartdatum is null)
            return true;

        return oldStartdatum is not null && newStartdatum is not null && newStartdatum.Equals(oldStartdatum);
    }

    public static implicit operator DateOnly?(Einddatum startdatum)
        => startdatum.Datum;

    public bool IsInFuture(DateOnly clockToday)
        => Datum > clockToday;

    public int CompareTo(DateOnly? other)
    {
        if (Datum.HasValue && other.HasValue)
            return Datum.Value.CompareTo(other.Value);

        if (!Datum.HasValue && !other.HasValue)
            return 0;

        return Datum.HasValue ? 1 : -1;
    }
}
