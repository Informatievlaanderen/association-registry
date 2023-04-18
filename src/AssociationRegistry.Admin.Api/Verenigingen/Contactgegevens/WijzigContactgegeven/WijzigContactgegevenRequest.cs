namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;

using System.Runtime.Serialization;
using Acties.WijzigContactgegeven;
using Vereniging;

[DataContract]
public class WijzigContactgegevenRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public RequestContactgegeven Contactgegeven { get; set; } = null!;

    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataContract]
    public class RequestContactgegeven
    {
        /// <summary>De waarde van het contactgegeven</summary>
        [DataMember(Name = "waarde")] public string? Waarde { get; set; } = null!;

        /// <summary>
        /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
        /// </summary>
        [DataMember(Name = "beschrijving")] public string? Beschrijving { get; set; } = null;

        /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
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
