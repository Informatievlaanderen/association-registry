namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;

using System.Runtime.Serialization;
using Vereniging.VoegContactgegevenToe;
using Vereniging.WijzigContactgegeven;

[DataContract]
public class WijzigContactgegevenRequest
{
    [DataMember(Name = "initiator")] public string Initiator { get; set; } = null!;
    [DataMember(Name = "contactgegeven")] public RequestContactgegeven Contactgegeven { get; set; } = null!;

    [DataContract]
    public class RequestContactgegeven
    {
        [DataMember(Name = "waarde")] public string? Waarde { get; set; } = null!;
        [DataMember(Name = "omschrijving")] public string? Omschrijving { get; set; } = null;

        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
        public bool? IsPrimair { get; set; }
    }

    public WijzigContactgegevenCommand ToCommand(string vCode, int contactgegevenId)
        => new(
            vCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                contactgegevenId,
                Contactgegeven.Waarde,
                Contactgegeven.Omschrijving,
                Contactgegeven.IsPrimair));
}
