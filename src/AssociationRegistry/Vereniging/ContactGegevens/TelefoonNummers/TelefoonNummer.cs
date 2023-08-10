namespace AssociationRegistry.Vereniging.TelefoonNummers;

using Framework;
using Exceptions;

public record TelefoonNummer(string Waarde):IContactWaarde
{
    public static readonly TelefoonNummer Leeg = new(string.Empty);

    private static readonly string[] AllowedCharacters = {
        " ", ".", "(", ")", "/", "-", "+",
    };



    public static TelefoonNummer Create(string? telefoonNummer)
    {
        if (string.IsNullOrEmpty(telefoonNummer))
            return Leeg;
        Throw<InvalidTelefoonNummerCharacter>.IfNot(IsNumber(Sanitize(telefoonNummer)));
        Throw<NoNumbersInTelefoonNummer>.IfNot(HasNumber(telefoonNummer));
        return new TelefoonNummer(telefoonNummer);
    }

    public static TelefoonNummer Hydrate(string telefoonNummer)
        => new(telefoonNummer);

    private static bool HasNumber(string telefoonNummer)
        => telefoonNummer.Any(char.IsNumber);

    private static string Sanitize(string telefoonNummer)
        => AllowedCharacters.Aggregate(telefoonNummer, (current, allowedCharacter) => current.Replace(allowedCharacter, string.Empty));

    private static bool IsNumber(string sanitizedNummer)
        => sanitizedNummer.All(char.IsNumber);
    public virtual bool Equals(IContactWaarde? other)
        => other?.Waarde == Waarde;
}
