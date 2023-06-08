namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;

using System.Runtime.Serialization;
using AssociationRegistry.Acties.WijzigContactgegeven;
using Vereniging;

[DataContract]
public class WijzigContactgegevenRequest
{
    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public TeWijzigenContactgegeven Contactgegeven { get; set; } = null!;

    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataContract]
    public class TeWijzigenContactgegeven
    {
        /// <summary>De waarde van het contactgegeven</summary>
        [DataMember(Name = "waarde")] public string? Waarde { get; set; } = null!;

        /// <summary>
        /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
        /// </summary>
        [DataMember(Name = "beschrijving")] public string? Beschrijving { get; set; } = null;

        /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
        [DataMember(Name = "isPrimair")]
        public bool? IsPrimair { get; set; }
    }

    public WijzigContactgegevenCommand ToCommand(string vCode, int contactgegevenId)
        => new(
            VCode.Create(vCode),
            new WijzigContactgegevenCommand.CommandContactgegeven(
                contactgegevenId,
                Contactgegeven.Waarde,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
