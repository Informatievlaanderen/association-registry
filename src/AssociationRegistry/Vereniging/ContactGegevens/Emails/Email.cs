﻿namespace AssociationRegistry.Vereniging.Emails;

using System.Text.RegularExpressions;
using Framework;
using Exceptions;

public record Email(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.Email, Waarde, Beschrijving, IsPrimair)
{
    private static readonly Regex EmailRegex = new(
        @"^(([a-z0-9]+[\.!#$%&'*+/=?^_`{|}~-]*)*[a-z0-9]+)@(([a-z0-9]+[\.-]?)*[a-z0-9]\.)+[a-z]{2,}$",
        RegexOptions.IgnoreCase);

    public static Email Create(string? email)
        => Create(email, string.Empty, false);

    public static Email Create(string? email, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(email))
            return null!;
        Throw<InvalidEmailFormat>.IfNot(MatchWithRegex(email));
        return new Email(email, beschrijving, isPrimair);
    }

    private static bool MatchWithRegex(string email)
        => EmailRegex.IsMatch(email);
}
