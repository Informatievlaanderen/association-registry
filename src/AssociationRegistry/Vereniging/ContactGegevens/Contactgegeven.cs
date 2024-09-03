﻿namespace AssociationRegistry.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Bronnen;
using Emails;
using Exceptions;
using Framework;
using SocialMedias;
using System.Text.RegularExpressions;
using TelefoonNummers;
using Websites;

public record Contactgegeven
{
    public const int MaxLengthBeschrijving = 42;

    protected Contactgegeven(Contactgegeventype contactgegeventype, string waarde, string beschrijving, bool isPrimair)
    {
        ContactgegevenId = 0;
        Contactgegeventype = contactgegeventype;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    private Contactgegeven(
        int contactgegevenId,
        Contactgegeventype contactgegeventype,
        string waarde,
        string beschrijving,
        bool isPrimair,
        Bron bron,
        ContactgegeventypeVolgensKbo? contactgegeventypeVolgensKbo = null)
    {
        ContactgegevenId = contactgegevenId;
        Contactgegeventype = contactgegeventype;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
        Bron = bron;
        TypeVolgensKbo = contactgegeventypeVolgensKbo;
    }

    public int ContactgegevenId { get; init; }
    public Contactgegeventype Contactgegeventype { get; init; }
    public string Waarde { get; init; }
    public string Beschrijving { get; init; }
    public bool IsPrimair { get; init; }
    public Bron Bron { get; init; }
    public ContactgegeventypeVolgensKbo? TypeVolgensKbo { get; set; }

    public virtual bool Equals(Contactgegeven? other)
    {
        if (ReferenceEquals(objA: null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return ContactgegevenId == other.ContactgegevenId &&
               Contactgegeventype.Equals(other.Contactgegeventype) &&
               Waarde == other.Waarde &&
               Beschrijving == other.Beschrijving &&
               IsPrimair == other.IsPrimair &&
               Bron == other.Bron;
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

    public static Contactgegeven Hydrate(
        int contactgegevenId,
        Contactgegeventype type,
        string waarde,
        string beschrijving,
        bool isPrimair,
        Bron bron,
        ContactgegeventypeVolgensKbo? typeVolgensKbo = null)
        => new(contactgegevenId, type, waarde, beschrijving, isPrimair, bron, typeVolgensKbo);

    public static Contactgegeven CreateFromInitiator(Contactgegeventype type, string waarde, string? beschrijving, bool isPrimair)
    {
        beschrijving ??= string.Empty;

        Contactgegeven contactgegeven = type switch
        {
            { Waarde: Contactgegeventype.Labels.Email } => Email.Create(waarde, beschrijving, isPrimair),
            { Waarde: Contactgegeventype.Labels.Telefoon } => TelefoonNummer.Create(waarde, beschrijving, isPrimair),
            { Waarde: Contactgegeventype.Labels.Website } => Website.Create(waarde, beschrijving, isPrimair),
            { Waarde: Contactgegeventype.Labels.SocialMedia } => SocialMedia.Create(waarde, beschrijving, isPrimair),
            _ => throw new ContactTypeIsOngeldig(),
        };

        return contactgegeven with { Bron = Bron.Initiator };
    }

    public static Contactgegeven CreateFromKbo(Contactgegeventype type, string waarde)
        => CreateFromInitiator(type, waarde, string.Empty, isPrimair: false) with { Bron = Bron.KBO };

    public static Contactgegeven Create(string type, string waarde, string? beschrijving, bool isPrimair)
    {
        Throw<ContactTypeIsOngeldig>.IfNot(IsKnownType(type));

        return CreateFromInitiator(Contactgegeventype.Parse(type), waarde, beschrijving, isPrimair);
    }

    public bool IsEquivalentTo(Contactgegeven contactgegeven)
        => Contactgegeventype == contactgegeven.Contactgegeventype
        && Beschrijving == contactgegeven.Beschrijving
        && (contactgegeven.Contactgegeventype == Contactgegeventype.Telefoon
               ? EqualsPhoneNumber(contactgegeven.Waarde)
               : Waarde == contactgegeven.Waarde);

    private static bool IsKnownType(string type)
        => Contactgegeventype.CanParse(type);

    public Contactgegeven CopyWithValuesIfNotNull(string? waarde, string? beschrijving, bool? isPrimair)
        => CreateFromInitiator(Contactgegeventype, waarde ?? Waarde, beschrijving ?? Beschrijving, isPrimair ?? IsPrimair) with
        {
            ContactgegevenId = ContactgegevenId,
            Bron = Bron,
        };

    public override int GetHashCode()
        => HashCode.Combine(ContactgegevenId, Contactgegeventype, Waarde, Beschrijving, IsPrimair, Bron);

    public bool WouldBeEquivalent(string? waarde, string? beschrijving, bool? isPrimair, out Contactgegeven contactgegeven)
    {
        contactgegeven = CopyWithValuesIfNotNull(waarde, beschrijving, isPrimair);

        return this == contactgegeven;
    }

    public static Contactgegeven? TryCreateFromKbo(string waarde, ContactgegeventypeVolgensKbo type)
    {
        try
        {
            return CreateFromKbo(type.Contactgegeventype, waarde);
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
