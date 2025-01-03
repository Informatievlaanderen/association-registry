namespace AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;

using AssociationRegistry.Acties.Dubbelbeheer.MarkeerAlsDubbelVan;
using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class MarkeerAlsDubbelVanRequest
{
    /// <summary>De VCode van de vereniging waarvan deze vereniging een dubbel is.</summary>
    [DataMember(Name = "isDubbelVan")]
    public string IsDubbelVan { get; set; } = null!;

    public MarkeerAlsDubbelVanCommand ToCommand(string vCode)
        => new(VCode.Create(vCode), VCode.Create(IsDubbelVan));
}
