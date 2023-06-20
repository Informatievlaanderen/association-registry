namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

/// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
public class MetadataDetail
{
    /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
    public string DatumLaatsteAanpassing { get; init; } = null!;

    /// <summary> De basis URI voor alle decentraal beheer acties die van toepassing zijn voor deze vereniging</summary>
    public string BeheerBasisUri { get; init; } = null!;
}