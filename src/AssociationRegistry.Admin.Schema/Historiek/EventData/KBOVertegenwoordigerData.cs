namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using System.Runtime.Serialization;

[DataContract]
public record KBOVertegenwoordigerData
(
    [property: DataMember]
    int VertegenwoordigerId,
    [property: DataMember]
    string Voornaam,
    [property: DataMember]
    string Achternaam
)
{
    public static KBOVertegenwoordigerData Create(VertegenwoordigerWerdOvergenomenUitKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam);

    public static KBOVertegenwoordigerData Create(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam);

    public static KBOVertegenwoordigerData Create(VertegenwoordigerWerdGewijzigdInKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam);

    public static KBOVertegenwoordigerData Create(VertegenwoordigerWerdVerwijderdUitKBO vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam);
}
