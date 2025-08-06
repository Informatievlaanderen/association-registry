namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;

public record VerenigingsNaam : IEquatable<string>
{
    private readonly string _naam;

    private VerenigingsNaam(string naam)
    {
        _naam = naam;
    }

    public static VerenigingsNaam Create(string naam)
    {
        Throw<VerenigingsnaamIsLeeg>.IfNullOrWhiteSpace(naam);

        return new VerenigingsNaam(naam);
    }

    internal static VerenigingsNaam Hydrate(string naam)
        => new(naam);

    public bool Equals(string? other)
        => _naam.Equals(other);

    public override string ToString()
        => _naam;

    public static implicit operator string(VerenigingsNaam naam)
        => naam.ToString();
}
