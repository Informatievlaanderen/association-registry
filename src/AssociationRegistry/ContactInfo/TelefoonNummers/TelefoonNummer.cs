namespace AssociationRegistry.ContactInfo.TelefoonNummers;

using Exceptions;
using Framework;

public record TelefoonNummer(string Value)
{
    private static readonly string[] AllowedCharacters =
    {
        " ", ".", "(", ")", "/", "-", "+",
    };

    public static TelefoonNummer Create(string? telefoonNummer)
    {
        if (string.IsNullOrEmpty(telefoonNummer))
            return null!;
        Throw<InvalidTelefoonNummerCharacter>.IfNot(IsNumber(Sanitize(telefoonNummer)));
        Throw<NoNumbersInTelefoonNummer>.IfNot(HasNumber(telefoonNummer));
        return new TelefoonNummer(telefoonNummer);
    }

    private static bool HasNumber(string telefoonNummer)
        => telefoonNummer.Any(char.IsNumber);

    private static string Sanitize(string telefoonNummer)
        => AllowedCharacters.Aggregate(telefoonNummer, (current, allowedCharacter) => current.Replace(allowedCharacter, string.Empty));

    private static bool IsNumber(string sanitizedNummer)
        => sanitizedNummer.All(char.IsNumber);

    public static implicit operator string?(TelefoonNummer? telefoonNummer)
        => telefoonNummer?.ToString();

    public static implicit operator TelefoonNummer(string? telefoonNummer)
        => Create(telefoonNummer);

    public override string ToString()
        => Value;
}
