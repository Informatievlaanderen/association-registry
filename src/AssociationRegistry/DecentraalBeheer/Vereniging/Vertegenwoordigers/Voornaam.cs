namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;

public record Voornaam
{
    private Voornaam()
    {
    }

    public string Waarde { get; init; } = null!;

    public static Voornaam Create(string waarde)
    {
        Throw<VoornaamBevatNummers>.If(waarde.Any(char.IsDigit));
        Throw<VoornaamZonderLetters>.IfNot(waarde.Any(char.IsLetter));

        return new Voornaam { Waarde = waarde };
    }

    public static implicit operator Voornaam(string waarde)
        => Create(waarde);

    public static implicit operator string(Voornaam voornaam)
        => voornaam.Waarde;

    public static Voornaam Hydrate(string waarde)
        => new() { Waarde = waarde };
}
