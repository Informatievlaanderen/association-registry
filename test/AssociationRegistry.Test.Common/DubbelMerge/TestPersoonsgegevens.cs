namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.Events;

/// <summary>
///     Elke persoon en elk bankrekeningnummer heeft andere waarden in elk veld, zodat een verwisseld of vergeten veld,
///     of een verkeerde RefId, opvalt.
/// </summary>
public static class TestVertegenwoordigers
{
    public static readonly Registratiedata.Vertegenwoordiger Frodo = new(
        VertegenwoordigerId: 1,
        Insz: "01010100123",
        IsPrimair: true,
        Roepnaam: "Underhill",
        Rol: "Voorzitter",
        Voornaam: "Frodo",
        Achternaam: "Baggins",
        Email: "frodo@example.org",
        Telefoon: "011223344",
        Mobiel: "0470112233",
        SocialMedia: "https://example.org/frodo"
    );

    public static readonly Registratiedata.Vertegenwoordiger Samwise = new(
        VertegenwoordigerId: 3,
        Insz: "02020200246",
        IsPrimair: false,
        Roepnaam: "Samwise the Brave",
        Rol: "Penningmeester",
        Voornaam: "Samwise",
        Achternaam: "Gamgee",
        Email: "samwise@example.org",
        Telefoon: "015667788",
        Mobiel: "0480445566",
        SocialMedia: "https://example.org/samwise"
    );

    public static readonly Registratiedata.Vertegenwoordiger Aragorn = new(
        VertegenwoordigerId: 2,
        Insz: "03030300369",
        IsPrimair: false,
        Roepnaam: "Strider",
        Rol: "Secretaris",
        Voornaam: "Aragorn",
        Achternaam: "Elessar",
        Email: "aragorn@example.org",
        Telefoon: "016998877",
        Mobiel: "0490778899",
        SocialMedia: "https://example.org/aragorn"
    );
}

public static class TestBankrekeningnummers
{
    public static readonly Registratiedata.Bankrekeningnummer BankrekeningnummerFrodo = new(
        BankrekeningnummerId: 1,
        Iban: "BE68539007547034",
        Doel: "Werking",
        Titularissen: ["Frodo Baggins"]
    );

    public static readonly Registratiedata.Bankrekeningnummer BankrekeningnummerSamwise = new(
        BankrekeningnummerId: 4,
        Iban: "BE71096123456769",
        Doel: "Evenementen",
        Titularissen: ["Samwise Gamgee", "Aragorn Elessar"]
    );

    public static readonly Registratiedata.Bankrekeningnummer BankrekeningnummerBilbo = new(
        BankrekeningnummerId: 2,
        Iban: "BE62510007547061",
        Doel: "Lidgelden",
        Titularissen: ["Bilbo Baggins"]
    );
}
