namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Runtime.Serialization;

/// <summary>De unieke identificator van het adres in een andere bron</summary>
[DataContract]
public class AdresId
{
    /// <summary>De code van de bron van het adres</summary>
    [DataMember]
    public string Broncode { get; set; } = null!;

    /// <summary>De unieke identificator volgens de bron</summary>
    [DataMember]
    public string Bronwaarde { get; set; } = null!;
}
