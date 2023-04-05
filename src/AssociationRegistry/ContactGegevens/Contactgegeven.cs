namespace AssociationRegistry.ContactGegevens;

using Emails;
using Exceptions;
using Framework;
using SocialMedias;
using TelefoonNummers;
using Websites;

public record Contactgegeven
{
    public int ContactgegevenId { get; init; }
    public ContactgegevenType Type { get; }
    public string Waarde { get; }
    public string Omschrijving { get; }
    public bool IsPrimair { get; }

    internal Contactgegeven(ContactgegevenType type, string waarde, string omschrijving, bool isPrimair)
    {
        ContactgegevenId = 0;
        Type = type;
        Waarde = waarde;
        Omschrijving = omschrijving;
        IsPrimair = isPrimair;
    }

    public static Contactgegeven Create(ContactgegevenType type, string waarde, string? omschrijving, bool isPrimair)
    {
        omschrijving ??= string.Empty;

        return type switch
        {
            { Waarde: nameof(ContactgegevenType.Email) } => Email.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.Telefoon) } => TelefoonNummer.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.Website) } => Website.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.SocialMedia) } => SocialMedia.Create(waarde, omschrijving, isPrimair),
            _ => throw new InvalidContactType(),
        };
    }

    public static Contactgegeven Create(string type, string waarde, string? omschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));
        return Create(ContactgegevenType.Parse(type), waarde, omschrijving, isPrimair);
    }

    private static bool IsKnownType(string type)
        => ContactgegevenType.TryParse(type, out _);


}
