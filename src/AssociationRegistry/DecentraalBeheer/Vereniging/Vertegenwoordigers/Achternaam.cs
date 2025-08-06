namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;

public record Achternaam
{
    private Achternaam()
    {
    }

    public Achternaam(string waarde) : this()
    {
        Waarde = waarde;
    }

    public string Waarde { get; init; } = null!;

    public static Achternaam Create(string waarde)
    {
        Throw<AchternaamBevatNummers>.If(waarde.Any(char.IsDigit));
        Throw<AchternaamZonderLetters>.IfNot(waarde.Any(char.IsLetter));

        return new Achternaam(waarde: waarde);
    }

    public static implicit operator Achternaam(string waarde)
        => Create(waarde);

    public static implicit operator string(Achternaam voornaam)
        => voornaam.Waarde;

    public static Achternaam Hydrate(string waarde)
        => new() { Waarde = waarde };
}
