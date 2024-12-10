namespace AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;

using Acties.MarkeerAlsDubbelVan;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class MarkeerAlsDubbelVanRequest
{
    /// <summary>De VCode van de vereniging waarvan deze vereniging een dubbel is.</summary>
    [DataMember(Name = "isDubbelVan")]
    public string IsDubbelVan { get; set; } = null!;

    public MarkeerAlsDubbelVanCommand ToCommand(string vCode)
        => new(VCode.Create(vCode), VCode.Create(IsDubbelVan));
}
