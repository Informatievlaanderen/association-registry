﻿namespace AssociationRegistry.ContactGegevens;

public sealed class ContactgegevenType : IEquatable<ContactgegevenType>
{
    public static readonly ContactgegevenType Email =
        new(
            nameof(Email)
        );

    public static readonly ContactgegevenType Website =
        new(
            nameof(Website)
        );

    public static readonly ContactgegevenType SocialMedia =
        new(
            nameof(SocialMedia)
        );

    public static readonly ContactgegevenType Telefoon =
        new(
            nameof(Telefoon)
        );

    public static readonly ContactgegevenType[] All =
    {
        Email, Website, SocialMedia, Telefoon,
    };

    public string Waarde { get; }

    private ContactgegevenType(string value)
    {
        Waarde = value;
    }


    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, candidate => candidate.Waarde == value) is not null;
    }

    public static bool TryParse(string? value, out ContactgegevenType parsed)
    {
        parsed = null!;
        if (value is null)
            return false;

        parsed = Array.Find(All, candidate => string.Equals(candidate.Waarde, value, StringComparison.InvariantCultureIgnoreCase)) ?? null!;
        return parsed is not null;
    }

    public static ContactgegevenType Parse(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!TryParse(value, out var parsed))
        {
            throw new FormatException($"De waarde {value} is geen gekend contactgegeven type.");
        }

        return parsed;
    }

    public bool Equals(ContactgegevenType? other)
        => other is not null && other.Waarde == Waarde;

    public override bool Equals(object? obj)
        => obj is ContactgegevenType type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(ContactgegevenType instance)
        => instance.ToString();

    public static bool operator ==(ContactgegevenType left, ContactgegevenType right)
        => Equals(left, right);

    public static bool operator !=(ContactgegevenType left, ContactgegevenType right)
        => !Equals(left, right);
}
