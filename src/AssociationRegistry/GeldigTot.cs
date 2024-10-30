namespace AssociationRegistry;

using System;

public struct GeldigTot: IEquatable<GeldigTot>, IComparable<GeldigTot>
{
    public DateOnly? DateOnly { get; }

    public bool IsInfinite
        => !DateOnly.HasValue;

    public GeldigTot(DateOnly? dateTime)
        => DateOnly = dateTime;

    public GeldigTot(int year, int month, int day)
        => DateOnly = new DateOnly(year, month, day);

    public static implicit operator DateOnly? (GeldigTot geldigTot)
        => geldigTot.DateOnly;

    public override string ToString()
        => DateOnly.HasValue ? DateOnly.Value.ToString("yyyy-MM-dd") : "~";

    public bool IsInFutureOf(DateOnly date, bool inclusive = false)
        => inclusive
            ? this >= new GeldigTot(date)
            : this > new GeldigTot(date);

    public bool IsInPastOf(DateOnly date, bool inclusive = false)
        => inclusive
            ? this <= new GeldigTot(date)
            : this < new GeldigTot(date);

    public static bool operator ==(GeldigTot left, GeldigTot right)
        => left.Equals(right);

    public static bool operator !=(GeldigTot left, GeldigTot right)
        => !(left == right);

    public static bool operator <(GeldigTot left, GeldigTot right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(GeldigTot left, GeldigTot right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(GeldigTot left, GeldigTot right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(GeldigTot left, GeldigTot right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(GeldigTot other)
    {
        if (!DateOnly.HasValue && !other.DateOnly.HasValue)
            return 0;

        if (!DateOnly.HasValue)
            return 1;

        if (!other.DateOnly.HasValue)
            return -1;

        return DateOnly.Value.CompareTo(other.DateOnly.Value);
    }

    public override bool Equals(object? obj)
        => obj is GeldigTot && Equals((GeldigTot) obj);

    public bool Equals(GeldigTot other)
        => DateOnly == other.DateOnly;

    public override int GetHashCode()
        => DateOnly.GetHashCode();
}
