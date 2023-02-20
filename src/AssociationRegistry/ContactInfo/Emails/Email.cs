namespace AssociationRegistry.ContactInfo.Emails;

using System.Text.RegularExpressions;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;
using Websites;

public class Email : StringValueObject<Website>
{
    private static readonly Regex EmailRegex = new(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");

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
