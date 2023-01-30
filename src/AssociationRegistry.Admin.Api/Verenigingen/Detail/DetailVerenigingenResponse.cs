namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record DetailVerenigingResponse(
    [property: DataMember(Name = "Vereniging")]
    DetailVerenigingResponse.VerenigingDetail Vereniging,
    [property: DataMember(Name = "Metadata")]
    DetailVerenigingResponse.MetadataDetail Metadata)
{
    [DataContract]
    public record VerenigingDetail(
        [property: DataMember(Name = "VCode")] string VCode,
        [property: DataMember(Name = "Naam")] string Naam,
        [property: DataMember(Name = "KorteNaam")]
        string? KorteNaam,
        [property: DataMember(Name = "KorteBeschrijving")]
        string? KorteBeschrijving,
        [property: DataMember(Name = "Startdatum")]
        DateOnly? Startdatum,
        [property: DataMember(Name = "KboNummer")]
        string? KboNummer,
        [property: DataMember(Name = "Status")]
        string Status,
        [property: DataMember(Name = "ContactInfoLijst")]
        VerenigingDetail.ContactInfo[] ContactInfoLijst,
        [property: DataMember(Name = "Locaties")]
        ImmutableArray<VerenigingDetail.Locatie> Locaties,
        [property: DataMember(Name = "Vertegenwoordigers")]
        ImmutableArray<VerenigingDetail.Vertegenwoordiger> Vertegenwoordigers)
    {
        [DataContract]
        public record ContactInfo(
            [property: DataMember(Name = "Contactnaam")]
            string? Contactnaam,
            [property: DataMember(Name = "Email")] string? Email,
            [property: DataMember(Name = "Telefoon")]
            string? Telefoon,
            [property: DataMember(Name = "Website")]
            string? Website,
            [property: DataMember(Name = "SocialMedia")]
            string? SocialMedia
        );

        [DataContract]
        public record Vertegenwoordiger(
            [property: DataMember(Name = "Rijksregisternummer")]
            string Rijksregisternummer,
            [property: DataMember(Name = "Voornaam")]
            string Voornaam,
            [property: DataMember(Name = "Achternaam")]
            string Achternaam,
            [property: DataMember(Name = "Roepnaam")]
            string? Roepnaam,
            [property: DataMember(Name = "Rol")] string? Rol,
            [property: DataMember(Name = "PrimairContactpersoon")]
            bool PrimairContactpersoon
        );

        [DataContract]
        public record Locatie(
            [property: DataMember(Name = "Locatietype")]
            string Locatietype,
            [property: DataMember(Name = "Hoofdlocatie", EmitDefaultValue = false)]
            bool Hoofdlocatie,
            [property: DataMember(Name = "Adres")] string Adres,
            [property: DataMember(Name = "Naam")] string? Naam,
            [property: DataMember(Name = "Straatnaam")]
            string Straatnaam,
            [property: DataMember(Name = "Huisnummer")]
            string Huisnummer,
            [property: DataMember(Name = "Busnummer")]
            string? Busnummer,
            [property: DataMember(Name = "Postcode")]
            string Postcode,
            [property: DataMember(Name = "Gemeente")]
            string Gemeente,
            [property: DataMember(Name = "Land")] string Land
        );
    }

    public record MetadataDetail(string DatumLaatsteAanpassing);
}
