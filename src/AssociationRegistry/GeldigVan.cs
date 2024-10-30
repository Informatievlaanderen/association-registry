namespace AssociationRegistry;

using System;

public struct GeldigVan: IEquatable<GeldigVan>, IComparable<GeldigVan>
{
    public DateOnly? DateOnly { get; }

    public bool IsInfinite
        => !DateOnly.HasValue;

    public GeldigVan(DateOnly? dateTime)
        => DateOnly = dateTime;

    public GeldigVan(int year, int month, int day)
        => DateOnly = new DateOnly(year, month, day);

    public static implicit operator DateOnly?(GeldigVan geldigVan)
        => geldigVan.DateOnly;

    public override string ToString()
        => DateOnly.HasValue? DateOnly.Value.ToString("yyyy-MM-dd") : "~";

    public bool IsInFutureOf(DateOnly date, bool inclusive = false)
        => inclusive
            ? this >= new GeldigVan(date)
            : this > new GeldigVan(date);

    public bool IsInPastOf(DateOnly date, bool inclusive = false)
        => inclusive
            ? this <= new GeldigVan(date)
            : this < new GeldigVan(date);

    public static bool operator ==(GeldigVan left, GeldigVan right)
        => left.Equals(right);

    public static bool operator !=(GeldigVan left, GeldigVan right)
        => !(left == right);

    public static bool operator <(GeldigVan left, GeldigVan right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(GeldigVan left, GeldigVan right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(GeldigVan left, GeldigVan right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(GeldigVan left, GeldigVan right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(GeldigVan other)
    {
        if (!DateOnly.HasValue && !other.DateOnly.HasValue)
            return 0;

        if (!DateOnly.HasValue)
            return -1;

        if (!other.DateOnly.HasValue)
            return 1;

        return DateOnly.Value.CompareTo(other.DateOnly.Value);
    }

    public override bool Equals(object? obj)
        => obj is GeldigVan && Equals((GeldigVan) obj);

    public bool Equals(GeldigVan other)
        => DateOnly == other.DateOnly;

    public override int GetHashCode()
        => DateOnly.GetHashCode();
}
