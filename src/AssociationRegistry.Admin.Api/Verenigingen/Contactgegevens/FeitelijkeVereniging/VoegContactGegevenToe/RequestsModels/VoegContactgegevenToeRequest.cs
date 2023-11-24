﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;

using Acties.VoegContactgegevenToe;
using Common;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class VoegContactgegevenToeRequest
{
    /// <summary>Het toe te voegen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public ToeTeVoegenContactgegeven Contactgegeven { get; set; } = null!;

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            AssociationRegistry.Vereniging.Contactgegeven.CreateFromInitiator(
                Contactgegeventype.Parse(Contactgegeven.Type),
                Contactgegeven.Waarde,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
