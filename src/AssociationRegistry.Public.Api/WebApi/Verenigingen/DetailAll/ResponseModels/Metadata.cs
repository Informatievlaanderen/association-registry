namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Metadata
{
    /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
    [DataMember(Name = "DatumLaatsteAanpassing")]
    public string DatumLaatsteAanpassing { get; init; } = null!;
}
