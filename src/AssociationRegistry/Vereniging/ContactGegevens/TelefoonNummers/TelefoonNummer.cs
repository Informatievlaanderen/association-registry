namespace AssociationRegistry.Vereniging.TelefoonNummers;

using Exceptions;
using Framework;
using System.Text.RegularExpressions;

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

    private bool EqualsPhoneNumber(string phoneNumber)
    {
        var originalPhoneNumber = NormalizePhoneNumber(Waarde);
        var otherPhoneNumber = NormalizePhoneNumber(phoneNumber);

        return originalPhoneNumber == otherPhoneNumber;
    }

    public static string NormalizePhoneNumber(string phoneNumber)
    {
        phoneNumber = phoneNumber.Replace("+", "00");

        var firstSpaceIndex = phoneNumber.IndexOf(" ");
        var phoneNumberDigits = Convert.ToInt64(Regex.Replace(phoneNumber, @"\D", "")).ToString();

        // Local number
        if (phoneNumberDigits.Length <= 9)
            return "0032" + phoneNumberDigits;

        // Country coded number
        if (firstSpaceIndex.Equals(-1)) // Does not use spaces inside number
            return Regex.Replace(phoneNumber, @"\D", "");

        var countryCode = phoneNumber.Substring(0, firstSpaceIndex);
        var countryCodeDigits = Regex.Replace(countryCode, @"\D", "");
        var phone = phoneNumber.Substring(firstSpaceIndex);
        var phoneDigits = Convert.ToInt64(Regex.Replace(phone, @"\D", "")).ToString();

        return countryCodeDigits + phoneDigits;
    }

    protected override bool CompareWaarde(string waarde)
        => EqualsPhoneNumber(waarde);

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
