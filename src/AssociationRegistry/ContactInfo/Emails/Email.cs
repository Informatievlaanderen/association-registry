namespace AssociationRegistry.ContactInfo.Emails;

using System.Text.RegularExpressions;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;
using Urls;

public class Email : StringValueObject<Url>
{
    private static readonly Regex EmailRegex = new(@"^([a-z]+[a-z0-9]*[\.!#$%&'*+/=?^_`{|}~-]?[a-z0-9]+)@(([a-z]+[a-z0-9]*[\.-]?)*[a-z0-9]{2,}\.)+[a-z]{2,}$");
    private Email(string @string) : base(@string)
    {
    }

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
