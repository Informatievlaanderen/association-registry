namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System;
using System.Runtime.Serialization;
using Projections.Historiek.Schema;

/// <summary>Alle gebeurtenissen van deze vereniging</summary>
[DataContract]
public class HistoriekResponse
{
    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "vCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Alle gebeurtenissen van deze vereniging</summary>
    [DataMember(Name = "gebeurtenissen")]
    public HistoriekGebeurtenisResponse[] Gebeurtenissen { get; init; } = Array.Empty<HistoriekGebeurtenisResponse>();
}

/// <summary>Een gebeurtenis van een vereniging</summary>
[DataContract]
public class HistoriekGebeurtenisResponse
{
    /// <summary>De beschrijving de gebeurtenis</summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; set; } = null!;

    /// <summary>Het type de gebeurtenis</summary>
    [DataMember(Name = "gebeurtenis")]
    public string Gebeurtenis { get; set; } = null!;

    /// <summary>De relevante data die hoort bij de gebeurtenis</summary>
    [DataMember(Name = "data")]
    public object? Data { get; set; }

    /// <summary>Instantie die de vereniging aanmaakt</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het tijdstip waarop de gebeurtenis plaatsvond</summary>
    [DataMember(Name = "tijdstip")]
    public string Tijdstip { get; set; } = null!;
}

public interface IHistoriekDataResponse
{
    static object? From(object? gebeurtenisData)
        => gebeurtenisData switch
        {
            VerenigingWerdgeregistreerdData data => new VerenigingWerdgeregistreerdDataResponse { Naam = data.Vereniging.Naam },
            NaamWerdGewijzigdData data => new NaamWerdGewijzigdDataResponse { Naam = data.Naam },
            KorteNaamWerdGewijzigdData data => new KorteNaamWerdGewijzigdDataResponse { KorteNaam = data.KorteNaam },
            KorteBeschrijvingWerdGewijzigdData data => new KorteBeschrijvingWerdGewijzigdDataResponse { KorteBeschrijving = data.KorteBeschrijving },
            StartdatumWerdGewijzigdData data => new StartdatumWerdGewijzigdDataResponse { Startdatum = data.StartDatum! },
             _ => gebeurtenisData,
        };
}

/// <summary>De relevante data voor het type gebeurtenis StartdatumWerdGewijzigd</summary>
[DataContract]
public class StartdatumWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    /// <summary>Datum waarop de vereniging gestart is</summary>
    [DataMember(Name = "startdatum")]
    public string Startdatum { get; set; } = null!;
}

/// <summary>De relevante data voor het type gebeurtenis KorteBeschrijvingWerdGewijzigd</summary>
[DataContract]
public class KorteBeschrijvingWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "korteBeschrijving")]
    public string KorteBeschrijving { get; set; } = null!;
}

/// <summary>De relevante data voor het type gebeurtenis KorteNaamWerdGewijzigd</summary>
[DataContract]
public class KorteNaamWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "korteNaam")]
    public string KorteNaam { get; set; } = null!;
}

/// <summary>De relevante data voor het type gebeurtenis NaamWerdGewijzigd</summary>
[DataContract]
public class NaamWerdGewijzigdDataResponse : IHistoriekDataResponse
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "naam")]
    public string Naam { get; set; } = null!;
}

/// <summary>De relevante data voor het type gebeurtenis VerenigingWerdgeregistreerd</summary>
[DataContract]
public class VerenigingWerdgeregistreerdDataResponse : IHistoriekDataResponse
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "naam")]
    public string Naam { get; set; } = null!;
}
