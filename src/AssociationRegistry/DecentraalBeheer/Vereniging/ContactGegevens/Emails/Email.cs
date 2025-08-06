namespace AssociationRegistry.DecentraalBeheer.Vereniging.Emails;

using Framework;
using Exceptions;
using System.Text.RegularExpressions;

public record Email(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(Contactgegeventype.Email, Waarde, Beschrijving, IsPrimair)
{
    public static readonly Email Leeg = new(string.Empty, string.Empty, IsPrimair: false);

    private static readonly Regex EmailRegex =
        new(pattern: @"^([_-]*([a-z0-9]+[\.!#$%&'*+/=?^_`{|}~-]?){1,}[_-]*)@(([a-z0-9]+[\.-]?)*[a-z0-9]\.)+[a-z]{2,}$",
            RegexOptions.IgnoreCase);

    public static Email Create(string? email)
        => Create(email, string.Empty, isPrimair: false);

    public static Email Create(string? email, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(email))
            return Leeg;

        Throw<EmailHeeftEenOngeldigFormaat>.IfNot(MatchWithRegex(email));

        return new Email(email, beschrijving, isPrimair);
    }

    public static Email Hydrate(string email)
        => new(email, string.Empty, IsPrimair: false);

    private static bool MatchWithRegex(string email)
        => EmailRegex.IsMatch(email);
}
