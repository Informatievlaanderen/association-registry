namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Exceptions;
using Extensions;
using Framework;

public record Titularissen
{
    public string[] Value { get; }

    private Titularissen(string[] value)
    {
        Value = value.ToArray();
    }

    public static Titularissen Create(string[] titularissen)
    {
        Throw<TitularisMagNietNullOfLeegZijn>.If(titularissen.IsNullOrEmpty());

        Throw<TitularisMagNietNullOfLeegZijn>.If(titularissen.HasNullOrWhiteSpaceValues());

        Throw<TitularissenMoetenUniekZijn>.If(titularissen.HasCaseInsensitiveDuplicateValues());

        return new Titularissen(titularissen);
    }

    public static Titularissen Hydrate(string[] titularissen) => new(titularissen.ToArray());

    public Titularissen Replace(string[]? titularissen) => titularissen == null ? this : Create(titularissen);

    public virtual bool Equals(Titularissen? other) =>
        other is not null && Value.SequenceEqual(other.Value, StringComparer.Ordinal);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var titularis in Value)
            hash.Add(titularis, StringComparer.Ordinal);

        return hash.ToHashCode();
    }
}
