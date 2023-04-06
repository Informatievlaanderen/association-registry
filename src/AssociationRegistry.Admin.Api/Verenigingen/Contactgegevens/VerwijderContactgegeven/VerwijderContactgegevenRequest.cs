namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerwijderContactgegeven;

using System.Runtime.Serialization;

[DataContract]
public class VerwijderContactgegevenRequest
{
    [DataMember(Name = "initiator")] public string Initiator { get; set; } = "";
}
