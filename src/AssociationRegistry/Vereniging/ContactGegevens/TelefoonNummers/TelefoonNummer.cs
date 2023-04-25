namespace AssociationRegistry.Vereniging.TelefoonNummers;

using Framework;
using Exceptions;

public record TelefoonNummer(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.Telefoon, Waarde, Beschrijving, IsPrimair)
{
    public static readonly TelefoonNummer Leeg = new(string.Empty, string.Empty, false);

    private static readonly string[] AllowedCharacters = {
        " ", ".", "(", ")", "/", "-", "+",
    };

    public static TelefoonNummer Create(string? telefoonNummer)
        => Create(telefoonNummer, string.Empty,false);

    public static TelefoonNummer Create(string? telefoonNummer, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(telefoonNummer))
            return Leeg;
        Throw<InvalidTelefoonNummerCharacter>.IfNot(IsNumber(Sanitize(telefoonNummer)));
        Throw<NoNumbersInTelefoonNummer>.IfNot(HasNumber(telefoonNummer));
        return new TelefoonNummer(telefoonNummer, beschrijving, isPrimair);
    }

    public static TelefoonNummer Hydrate(string telefoonNummer)
        => new(telefoonNummer, string.Empty,false);

    private static bool HasNumber(string telefoonNummer)
        => telefoonNummer.Any(char.IsNumber);

    private static string Sanitize(string telefoonNummer)
        => AllowedCharacters.Aggregate(telefoonNummer, (current, allowedCharacter) => current.Replace(allowedCharacter, string.Empty));

    private static bool IsNumber(string sanitizedNummer)
        => sanitizedNummer.All(char.IsNumber);
}
