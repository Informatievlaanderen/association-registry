namespace AssociationRegistry.Admin.Api.Hoofdactiviteiten.ResponseModels;

public class HoofdactiviteitVerenigingsloketResponse
{
    /// <summary>
    /// De verkorte code van de hoofdactiviteit
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van de hoofdactiviteit
    /// </summary>
    public string Beschrijving { get; set; } = null!;
}
