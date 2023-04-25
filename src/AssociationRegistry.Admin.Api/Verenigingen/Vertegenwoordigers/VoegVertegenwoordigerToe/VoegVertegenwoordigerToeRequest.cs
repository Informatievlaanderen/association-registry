namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;

using System;
using System.Runtime.Serialization;
using Acties.VoegContactgegevenToe;
using Common;

[DataContract]
public class VoegVertegenwoordigerToeRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het toe te voegen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public ToeTeVoegenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => throw new NotImplementedException();
}
