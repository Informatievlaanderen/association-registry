namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerwijderContactgegeven;

using System.Runtime.Serialization;

[DataContract]
public class VerwijderContactgegevenRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = string.Empty;
}
