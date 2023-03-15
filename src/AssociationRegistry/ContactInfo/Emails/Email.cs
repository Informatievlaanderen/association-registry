namespace AssociationRegistry.ContactInfo.Emails;

using System.Text.RegularExpressions;
using Exceptions;
using Framework;

public record Email(string Value)
{
    private static readonly Regex EmailRegex = new(
        @"^(([a-z0-9]+[\.!#$%&'*+/=?^_`{|}~-]*)*[a-z0-9]+)@(([a-z0-9]+[\.-]?)*[a-z0-9]\.)+[a-z]{2,}$",
        RegexOptions.IgnoreCase);


    public static Email Create(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return null!;
        Throw<InvalidEmailFormat>.IfNot(MatchWithRegex(email));
        return new Email(email);
    }

    private static bool MatchWithRegex(string email)
        => EmailRegex.IsMatch(email);

    public static implicit operator string?(Email? email)
        => email?.ToString();

    public static implicit operator Email(string? email)
        => Create(email);

    public override string ToString()
        => Value;
}
