namespace AssociationRegistry.ContactInfo.TelefoonNummers;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class TelefoonNummer : StringValueObject<TelefoonNummer>
{
    private static readonly string[] AllowedCharacters = new[]
    {
        " ", ".", "(", ")", "/", "-",
    };

    private TelefoonNummer(string @string) : base(@string)
    {
    }

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
