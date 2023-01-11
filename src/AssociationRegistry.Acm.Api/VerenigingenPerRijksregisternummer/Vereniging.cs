namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

using System.Runtime.Serialization;

[DataContract]
public record Vereniging(
    [property: DataMember] string VCode,
    [property: DataMember] string Naam);
