namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;

using System.Runtime.Serialization;

/// <summary>De unieke identificator van het adres in een andere bron</summary>
[DataContract]
public class AdresId
{
    /// <summary>De code van de bron van het adres</summary>
    [DataMember (Name = "broncode")]
    public string Broncode { get; set; } = null!;

    /// <summary>De unieke identificator volgens de bron</summary>
    [DataMember (Name = "bronwaarde")]
    public string Bronwaarde { get; set; } = null!;
}
