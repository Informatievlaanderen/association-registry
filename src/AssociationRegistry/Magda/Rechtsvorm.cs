namespace AssociationRegistry.Magda;

using System.Diagnostics.CodeAnalysis;
using Vereniging;

public class Rechtsvorm : IEquatable<Rechtsvorm>
{
    public static readonly Rechtsvorm VZW = new("017", Verenigingstype.VZW);
    public static readonly Rechtsvorm IVZW = new("125", Verenigingstype.IVZW);
    public static readonly Rechtsvorm PrivateStichting = new("026", Verenigingstype.PrivateStichting);
    public static readonly Rechtsvorm StichtingVanOpenbaarNut = new("029", Verenigingstype.StichtingVanOpenbaarNut);

    public static Rechtsvorm[] All = { VZW, IVZW, PrivateStichting, StichtingVanOpenbaarNut };

    public static readonly RechtsvormBron UitMagda = (rechtsvorm, waarde) => rechtsvorm.CodeVolgensMagda == waarde;

    public string CodeVolgensMagda { get; }
    public Verenigingstype Verenigingstype { get; }

    private Rechtsvorm( string codeVolgensMagda, Verenigingstype verenigingstype)
    {
        CodeVolgensMagda = codeVolgensMagda;
        Verenigingstype = verenigingstype;
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
        => other is not null && other.Verenigingstype.Equals(Verenigingstype);

    public override bool Equals(object? obj)
        => obj is Rechtsvorm type && Equals(type);

    public override int GetHashCode()
        => Verenigingstype.GetHashCode();

    public override string ToString()
        => Verenigingstype.ToString();

    public static bool operator ==(Rechtsvorm left, Rechtsvorm right)
        => Equals(left, right);

    public static bool operator !=(Rechtsvorm left, Rechtsvorm right)
        => !(left == right);

    public static bool operator ==(Rechtsvorm left, string right)
        => left.Verenigingstype.Equals(Verenigingstype.Parse(right));

    public static bool operator !=(Rechtsvorm left, string right)
        => !(left == right);

    public static bool operator ==(string left, Rechtsvorm right)
        => right == left;

    public static bool operator !=(string left, Rechtsvorm right)
        => !(left == right);
}

public delegate bool RechtsvormBron(Rechtsvorm rechtsvorm, string waarde);
