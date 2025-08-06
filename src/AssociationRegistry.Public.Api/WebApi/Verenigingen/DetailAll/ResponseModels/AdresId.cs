namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

public class AdresId
{
    /// <summary>De identificator voor dit adres bij de Adresbron</summary>
    [DataMember(Name = "Broncode")]
    public string? Broncode { get; init; }

    /// <summary>De bron waar het AdresId naar verwijst</summary>
    [DataMember(Name = "Bronwaarde")]
    public string? Bronwaarde { get; init; }
}
