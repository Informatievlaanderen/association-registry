namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class SearchVerenigingenResponse
{
    /// <summary>De JSON-LD open data context</summary>
    [DataMember(Name = "@context")]
    public string Context { get; init; }= null!;

    /// <summary>
    /// Dit is de lijst van verenigingen die het resultaat van de zoekopdracht zijn
    /// </summary>
    [DataMember(Name = "Verenigingen")]
    public Vereniging[] Verenigingen { get; set; } = null!;

    /// <summary>
    /// Facets bevatten extra contextuele informatie over de zoekresultaten wat kan gebruikt worden voor verdere verfijning van de zoekopdracht
    /// </summary>
    [DataMember(Name = "Facets")]
    public Facets? Facets { get; set; }

    /// <summary>
    /// In deze metadata plaatsen we alle relevante metadata voor de zoekopdracht, de paginering informatie
    /// </summary>
    [DataMember(Name = "Metadata")]
    public Metadata? Metadata { get; set; }
}
