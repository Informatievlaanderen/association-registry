namespace AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;

using System.Runtime.Serialization;
using Constants;
using ContactGegevens;
using Vereniging.VoegContactgegevenToe;

[DataContract]
public class VoegContactgegevenToeRequest
{
    [DataMember(Name = "initiator")] public string Initiator { get; set; } = null!;
    [DataMember(Name = "contactgegeven")] public RequestContactgegeven Contactgegeven { get; set; } = null!;

    [DataContract]
    public class RequestContactgegeven
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "waarde")] public string Waarde { get; set; } = null!;
        [DataMember(Name = "omschrijving")] public string? Omschrijving { get; set; } = null;

        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
        public bool IsPrimair { get; set; }
    }

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => new(
            vCode,
            new VoegContactgegevenToeCommand.CommandContactgegeven(
                ContactgegevenType.Parse(Contactgegeven.Type),
                Contactgegeven.Waarde,
                Contactgegeven.Omschrijving,
                Contactgegeven.IsPrimair));
}
