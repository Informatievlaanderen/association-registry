namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using Events.Enriched;
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
    private const string VerlopenPersoonsgegevenText = "<Onbekend>";

    public static VertegenwoordigerData Create(VertegenwoordigerWerdToegevoegdMetPersoonsgegevens e)
        => new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Roepnaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Rol),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Voornaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Achternaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Email),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Telefoon),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Mobiel),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.SocialMedia));

    private static string DataOfOnbekend(string? data)
    {
        return data ?? VerlopenPersoonsgegevenText;
    }

    public static VertegenwoordigerData Create(VertegenwoordigerWerdGewijzigdMetPersoonsgegevens e)
        => new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Roepnaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Rol),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Voornaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Achternaam),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Email),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Telefoon),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.Mobiel),
            DataOfOnbekend(e.VertegenwoordigerPersoonsgegevens.SocialMedia));

    public static VertegenwoordigerData Create(EnrichedVertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.IsPrimair,
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Roepnaam),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Rol),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Voornaam),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Achternaam),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Email),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Telefoon),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.Mobiel),
            DataOfOnbekend(vertegenwoordiger.VertegenwoordigerPersoonsgegevens.SocialMedia));

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

    public static VertegenwoordigerData Create(VertegenwoordigerWerdGewijzigdInKBO vertegenwoordiger)
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

    public static VertegenwoordigerData Create(VertegenwoordigerWerdVerwijderdUitKBO vertegenwoordiger)
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
