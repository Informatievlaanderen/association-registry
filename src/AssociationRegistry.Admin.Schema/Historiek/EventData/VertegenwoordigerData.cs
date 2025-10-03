namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using System.Runtime.Serialization;

[DataContract]
public record VertegenwoordigerData
(
    [property: DataMember]
    int VertegenwoordigerId,
    [property: DataMember]
    bool IsPrimair,
    [property: DataMember]
    string Roepnaam,
    [property: DataMember]
    string Rol,
    [property: DataMember]
    string Voornaam,
    [property: DataMember]
    string Achternaam,
    [property: DataMember(Name = "E-mail")]
    string Email,
    [property: DataMember]
    string Telefoon,
    [property: DataMember]
    string Mobiel,
    [property: DataMember]
    string SocialMedia
)
{
    public static VertegenwoordigerData Create(VertegenwoordigerWerdToegevoegd e)
        => new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            e.Roepnaam,
            e.Rol,
            e.Voornaam,
            e.Achternaam,
            e.Email,
            e.Telefoon,
            e.Mobiel,
            e.SocialMedia);

    public static VertegenwoordigerData Create(VertegenwoordigerWerdGewijzigd e)
        => new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            e.Roepnaam,
            e.Rol,
            e.Voornaam,
            e.Achternaam,
            e.Email,
            e.Telefoon,
            e.Mobiel,
            e.SocialMedia);

    public static VertegenwoordigerData Create(Registratiedata.Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email,
            vertegenwoordiger.Telefoon,
            vertegenwoordiger.Mobiel,
            vertegenwoordiger.SocialMedia);

    public static VertegenwoordigerData Create(VertegenwoordigerWerdOvergenomenUitKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               IsPrimair: false,
               string.Empty,
               string.Empty,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam,
               string.Empty,
               string.Empty,
               string.Empty,
               string.Empty);

    public static VertegenwoordigerData Create(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               IsPrimair: false,
               string.Empty,
               string.Empty,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam,
               string.Empty,
               string.Empty,
               string.Empty,
               string.Empty);
}
