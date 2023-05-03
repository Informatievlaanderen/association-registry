namespace AssociationRegistry.Vereniging.Emails;

using System.Text.RegularExpressions;
using Framework;
using Exceptions;

public record Email(string Waarde)
{
    public static readonly Email Leeg = new(string.Empty);

    private static readonly Regex EmailRegex = new(
        @"^(([a-z0-9]+[\.!#$%&'*+/=?^_`{|}~-]*)*[a-z0-9]+)@(([a-z0-9]+[\.-]?)*[a-z0-9]\.)+[a-z]{2,}$",
        RegexOptions.IgnoreCase);

    public static Email Create(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return Leeg;
        Throw<InvalidEmailFormat>.IfNot(MatchWithRegex(email));
        return new Email(email);
    }

    public static Email Hydrate(string email)
        => new(email);

    public override string ToString()
        => Waarde;

    private static bool MatchWithRegex(string email)
        => EmailRegex.IsMatch(email);
}
