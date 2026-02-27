namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Bankrekeningnummer
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// De unieke identificatie code van dit bankrekeningnummer binnen de vereniging
    /// </summary>
    [DataMember(Name = "BankrekeningnummerId")]
    public int BankrekeningnummerId { get; set; }

    /// <summary>
    /// De IBAN van het bankrekeningnummer
    /// </summary>
    [DataMember(Name = "IBAN")]
    public string Iban { get; set; } = null!;

    /// <summary>
    /// Waar deze rekening voor gebruikt wordt
    /// </summary>
    [DataMember(Name = "Doel")]
    public string Doel { get; set; } = null!;

    /// <summary>
    /// De titularis van het bankrekeningnummer
    /// </summary>
    [DataMember(Name = "Titularis")]
    public string Titularis { get; set; } = null!;

    /// <summary>
    /// Gegevens initiatoren die dit bankrekeningnummer bevestigd hebben
    /// </summary>
    [DataMember(Name = "BevestigdDoor")]
    public Gegevensinitiator[] BevestigdDoor { get; set; } = [];

    /// <summary> De bron die dit bankrekeningnummer beheert.
    /// <br />
    ///     Mogelijke waarden:<br />
    ///     - Initiator<br />
    ///     - KBO
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; set; } = null!;
}

public record Gegevensinitiator(string OvoCode);
