namespace AssociationRegistry.Vereniging.TelefoonNummers;

using Exceptions;
using Framework;

public record TelefoonNummer(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(Contactgegeventype.Telefoon, Waarde, Beschrijving, IsPrimair)
{
    public static readonly TelefoonNummer Leeg = new(string.Empty, string.Empty, IsPrimair: false);

    private static readonly string[] AllowedCharacters =
    {
        " ", ".", "(", ")", "/", "-", "+",
    };

    public static TelefoonNummer Create(string? telefoonNummer)
        => Create(telefoonNummer, string.Empty, isPrimair: false);

    public static TelefoonNummer Create(string? telefoonNummer, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(telefoonNummer))
            return Leeg;

        Throw<TelefoonNummerBevatOngeldigeTekens>.IfNot(IsNumber(Sanitize(telefoonNummer)));
        Throw<TelefoonNummerMoetCijferBevatten>.IfNot(HasNumber(telefoonNummer));

        return new TelefoonNummer(telefoonNummer, beschrijving, isPrimair);
    }

    public static TelefoonNummer Hydrate(string telefoonNummer)
        => new(telefoonNummer, string.Empty, IsPrimair: false);

    private static bool HasNumber(string telefoonNummer)
        => telefoonNummer.Any(char.IsNumber);

    private static string Sanitize(string telefoonNummer)
        => AllowedCharacters.Aggregate(telefoonNummer,
                                       func: (current, allowedCharacter) => current.Replace(allowedCharacter, string.Empty));

    private static bool IsNumber(string sanitizedNummer)
        => sanitizedNummer.All(char.IsNumber);
}
