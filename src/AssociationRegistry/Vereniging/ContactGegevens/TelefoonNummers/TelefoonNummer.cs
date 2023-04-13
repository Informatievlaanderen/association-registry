namespace AssociationRegistry.Vereniging.TelefoonNummers;

using Framework;
using Exceptions;

public record TelefoonNummer(string Waarde, string Omschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.Telefoon, Waarde, Omschrijving, IsPrimair)
{
    private static readonly string[] AllowedCharacters = {
        " ", ".", "(", ")", "/", "-", "+",
    };

    public static TelefoonNummer Create(string? telefoonNummer)
        => Create(telefoonNummer, string.Empty,false);

    public static TelefoonNummer Create(string? telefoonNummer, string omschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(telefoonNummer))
            return null!;
        Throw<InvalidTelefoonNummerCharacter>.IfNot(IsNumber(Sanitize(telefoonNummer)));
        Throw<NoNumbersInTelefoonNummer>.IfNot(HasNumber(telefoonNummer));
        return new TelefoonNummer(telefoonNummer, omschrijving, isPrimair);
    }

    private static bool HasNumber(string telefoonNummer)
        => telefoonNummer.Any(char.IsNumber);

    private static string Sanitize(string telefoonNummer)
        => AllowedCharacters.Aggregate(telefoonNummer, (current, allowedCharacter) => current.Replace(allowedCharacter, string.Empty));

    private static bool IsNumber(string sanitizedNummer)
        => sanitizedNummer.All(char.IsNumber);
}
