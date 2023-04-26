namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;

using System.Runtime.Serialization;
using Acties.VerwijderVertegenwoordiger;
using Vereniging;

[DataContract]
public class VerwijderVertegenwoordigerRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = string.Empty;

    public VerwijderVertegenwoordigerCommand ToCommand(string vCode, int vertegenwoordigerId)
        => new(VCode.Create(vCode), vertegenwoordigerId);
}
