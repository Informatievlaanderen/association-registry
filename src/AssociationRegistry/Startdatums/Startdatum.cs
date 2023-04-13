namespace AssociationRegistry.Startdatums;

public record Startdatum(DateOnly? Datum)
{
    public static readonly Startdatum Leeg = new((DateOnly?)null);

    public bool IsLeeg
        => Equals(Leeg);

    public static Startdatum Create(DateOnly? startdatum)
        => new(startdatum);

    public static Startdatum Hydrate(DateOnly? dateOnly)
        => new(dateOnly);

    public static bool Equals(Startdatum? oldStartdatum, Startdatum? newStartdatum)
    {
        if (oldStartdatum is null && newStartdatum is null)
            return true;

        return oldStartdatum is not null && newStartdatum is not null && newStartdatum.Equals(oldStartdatum);
    }

    public static implicit operator DateOnly?(Startdatum startdatum)
        => startdatum.Datum;

    public bool IsInFuture(DateOnly clockToday)
        => Datum > clockToday;
}
