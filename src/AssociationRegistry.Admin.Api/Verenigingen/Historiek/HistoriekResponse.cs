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
public class VerenigingWerdgeregistreerdDataResponse : IHistoriekDataResponse
{
    [DataMember(Name = "naam")] public string Naam { get; set; } = null!;
}
