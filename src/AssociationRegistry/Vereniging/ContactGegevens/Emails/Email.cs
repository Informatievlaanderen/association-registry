namespace AssociationRegistry.Vereniging.Emails;

using System.Text.RegularExpressions;
using Framework;
using Exceptions;

public record Email(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.Email, Waarde, Beschrijving, IsPrimair)
{
    public static readonly Email Leeg = new(string.Empty, string.Empty, false);

    private static readonly Regex EmailRegex = new(
        @"^(([a-z0-9]+[\.!#$%&'*+/=?^_`{|}~-]*)*[a-z0-9]+)@(([a-z0-9]+[\.-]?)*[a-z0-9]\.)+[a-z]{2,}$",
        RegexOptions.IgnoreCase);

    public static Email Create(string? email)
        => Create(email, string.Empty, false);


    public static Email Create(string? email, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(email))
            return Leeg;
        Throw<InvalidEmailFormat>.IfNot(MatchWithRegex(email));
        return new Email(email, beschrijving, isPrimair);
    }

    public static Email Hydrate(string email)
        => new(email, string.Empty, false);

    private static bool MatchWithRegex(string email)
        => EmailRegex.IsMatch(email);
}
