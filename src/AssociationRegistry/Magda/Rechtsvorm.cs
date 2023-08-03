namespace AssociationRegistry.Magda;

using System.Diagnostics.CodeAnalysis;

public class Rechtsvorm : IEquatable<Rechtsvorm>
{
    public static readonly Rechtsvorm VZW = new("vzw", "017");
    public static readonly Rechtsvorm IVZW = new("ivzw", "125");
    public static readonly Rechtsvorm PrivateStichting = new("private stichting", "026");
    public static readonly Rechtsvorm StichtingVanOpenbaarNut = new("stichting van openbaar nut", "029");

    public static Rechtsvorm[] All = { VZW, IVZW, PrivateStichting, StichtingVanOpenbaarNut };

    public static readonly RechtsvormBron UitVr = (rechtsvorm, waarde) => rechtsvorm.Waarde == waarde;
    public static readonly RechtsvormBron UitMagda = (rechtsvorm, waarde) => rechtsvorm.CodeVolgensMagda == waarde;

    public string Waarde { get; }
    public string CodeVolgensMagda { get; }

    private Rechtsvorm(string value, string codeVolgensMagda)
    {
        Waarde = value;
        CodeVolgensMagda = codeVolgensMagda;
    }


    public static bool CanParse(RechtsvormBron rechtsvormBron, string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, candidate => rechtsvormBron(candidate, value)) is not null;
    }

    public static bool TryParse(RechtsvormBron rechtsvormBron, string? value, [NotNullWhen(true)] out Rechtsvorm? parsed)
    {
        parsed = null;
        if (value is null)
            return false;

        parsed = Array.Find(All, candidate => rechtsvormBron(candidate, value)) ?? null;
        return parsed is not null;
    }

    public static Rechtsvorm Parse(RechtsvormBron rechtsvormBron, string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!TryParse(rechtsvormBron, value, out var parsed))
        {
            throw new FormatException($"De waarde {value} is geen gekende rechtsvorm.");
        }

        return parsed;
    }

    public bool Equals(Rechtsvorm? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object? obj)
        => obj is Rechtsvorm type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static bool operator ==(Rechtsvorm left, Rechtsvorm right)
        => Equals(left, right);

    public static bool operator !=(Rechtsvorm left, Rechtsvorm right)
        => !(left == right);

    public static bool operator ==(Rechtsvorm left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(Rechtsvorm left, string right)
        => !(left == right);

    public static bool operator ==(string left, Rechtsvorm right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, Rechtsvorm right)
        => !(left == right);
}

public delegate bool RechtsvormBron(Rechtsvorm rechtsvorm, string waarde);
