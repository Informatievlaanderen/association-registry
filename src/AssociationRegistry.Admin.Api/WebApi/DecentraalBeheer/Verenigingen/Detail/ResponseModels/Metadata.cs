namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

/// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
public class Metadata
{
    /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
    public string DatumLaatsteAanpassing { get; init; } = null!;
}
