namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System;
using System.Runtime.Serialization;
using Projections.Historiek.Schema;

[DataContract]
public class HistoriekResponse
{
    [DataMember(Name = "vCode")] public string VCode { get; init; } = null!;

    [DataMember(Name = "gebeurtenissen")] public HistoriekGebeurtenisResponse[] Gebeurtenissen { get; init; } = Array.Empty<HistoriekGebeurtenisResponse>();
}

[DataContract]
public class HistoriekGebeurtenisResponse
{
    [DataMember(Name = "beschrijving")] public string Beschrijving { get; set; } = null!;

    [DataMember(Name = "gebeurtenis")] public string Gebeurtenis { get; set; } = null!;

    [DataMember(Name = "data")] public object? Data { get; set; }

    [DataMember(Name = "initiator")] public string Initiator { get; set; } = null!;

    [DataMember(Name = "tijdstip")] public string Tijdstip { get; set; } = null!;
}

public interface IHistoriekDataResponse
{
    static object? From(object? gebeurtenisData)
    {
        return gebeurtenisData switch
        {
            VerenigingWerdgeregistreerdData data => new VerenigingWerdgeregsitreerdDataResponse { Naam = data.Vereniging.Naam },
            NaamWerdGewijzigdData data => new NaamWerdGewijzigdDataResponse { Naam = data.Naam },
            KorteNaamWerdGewijzigdData data => new KorteNaamWerdGewijzigdDataResponse { KorteNaam = data.KorteNaam },
            StartdatumWerdGewijzigdData data => new StartdatumWerdGewijzigdDataResponse { Startdatum = data.StartDatum },
            EmailContactInfoWerdGewijzigdHistoriekData data => new EmailContactInfoWerdGewijzigdHistoriekDataResponse { Contactnaam = data.Contactnaam, Email = data.Email },
            TelefoonContactInfoWerdGewijzigdHistoriekData data => new TelefoonContactInfoWerdGewijzigdHistoriekDataResponse { Contactnaam = data.Contactnaam, Telefoon = data.Telefoon },
            WebsiteContactInfoWerdGewijzigdHistoriekData data => new WebsiteContactInfoWerdGewijzigdHistoriekDataResponse { Contactnaam = data.Contactnaam, Website = data.Website },
            SocialMediaContactInfoWerdGewijzigdHistoriekData data => new SocialMediaContactInfoWerdGewijzigdHistoriekDataResponse { Contactnaam = data.Contactnaam, SocialMedia = data.SocialMedia },
            PrimairContactInfoWerdGewijzigdHistoriekData data => new PrimairContactInfoWerdGewijzigdHistoriekDataResponse { Contactnaam = data.Contactnaam, Primair = data.Primair },
            ContactInfoWerdToegevoegdData data => new ContactInfoWerdToegevoegdDataResponse { Contactnaam = data.ContactInfo.Contactnaam, Email = data.ContactInfo.Email, Telefoon = data.ContactInfo.Telefoon, Website = data.ContactInfo.Website, SocialMedia = data.ContactInfo.SocialMedia, Primair = data.ContactInfo.PrimairContactInfo },
            ContactInfoWerdVerwijderdData data => new ContactInfoWerdVerwijderdDataResponse { Contactnaam = data.Contactnaam },
            _ => gebeurtenisData,
        };
    }
}

[DataContract]
public class ContactInfoWerdVerwijderdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
}

[DataContract]
public class ContactInfoWerdToegevoegdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "email")] public string? Email { get; set; }
    [DataMember(Name = "telefoon")] public string? Telefoon { get; set; }
    [DataMember(Name = "website")] public string? Website { get; set; }
    [DataMember(Name = "socialMedia")] public string? SocialMedia { get; set; }
    [DataMember(Name = "primair")] public bool Primair { get; set; }
}

[DataContract]
public class PrimairContactInfoWerdGewijzigdHistoriekDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "primair")] public bool Primair { get; set; }
}

[DataContract]
public class SocialMediaContactInfoWerdGewijzigdHistoriekDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "socialMedia")] public string SocialMedia { get; set; } = null!;
}

[DataContract]
public class WebsiteContactInfoWerdGewijzigdHistoriekDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "website")] public string Website { get; set; } = null!;
}

[DataContract]
public class TelefoonContactInfoWerdGewijzigdHistoriekDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "telefoon")] public string Telefoon { get; set; } = null!;
}

[DataContract]
public class EmailContactInfoWerdGewijzigdHistoriekDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "contactnaam")] public string Contactnaam { get; set; } = null!;
    [DataMember(Name = "email")] public string Email { get; set; } = null!;
}

[DataContract]
public class StartdatumWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "startdatum")] public string Startdatum { get; set; } = null!;
}

[DataContract]
public class KorteBeschrijvingWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "korteBeschrijving")]
    public string KorteBeschrijving { get; set; } = null!;
}

[DataContract]
public class KorteNaamWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "korteNaam")] public string KorteNaam { get; set; } = null!;
}

[DataContract]
public class NaamWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "naam")] public string Naam { get; set; } = null!;
}

[DataContract]
public class VerenigingWerdgeregsitreerdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "naam")] public string Naam { get; set; } = null!;
}
