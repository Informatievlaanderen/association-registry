namespace AssociationRegistry.ContactGegevens;

using Emails;
using Exceptions;
using Framework;
using SocialMedias;
using TelefoonNummers;
using Websites;

public record Contactgegeven
{
    public ContactgegevenType Type { get; }
    public string Waarde { get; }
    public string Omschrijving { get; }
    public bool IsPrimair { get; }

    internal Contactgegeven(ContactgegevenType type, string waarde, string omschrijving, bool isPrimair)
    {
        Type = type;
        Waarde = waarde;
        Omschrijving = omschrijving;
        IsPrimair = isPrimair;
    }

    public static Contactgegeven Create(string type, string waarde, string? omschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));

        omschrijving ??= string.Empty;
        return Enum.Parse<ContactgegevenType>(type, true) switch
        {
            ContactgegevenType.Email => Email.Create(waarde, omschrijving,isPrimair),
            ContactgegevenType.Telefoon => TelefoonNummer.Create(waarde, omschrijving,isPrimair),
            ContactgegevenType.Website => Website.Create(waarde, omschrijving,isPrimair),
            ContactgegevenType.SocialMedia => SocialMedia.Create(waarde, omschrijving,isPrimair),
            _ => throw new InvalidContactType(),
        };
    }

    private static bool IsKnownType(string type)
        => Enum.TryParse<ContactgegevenType>(type, true, out _);

    public enum ContactgegevenType
    {
        Email,
        Website,
        SocialMedia,
        Telefoon,
    }
}
