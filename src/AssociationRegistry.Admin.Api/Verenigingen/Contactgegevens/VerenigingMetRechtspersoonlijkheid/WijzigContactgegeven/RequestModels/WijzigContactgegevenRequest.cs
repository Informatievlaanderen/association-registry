﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;

using Acties.WijzigContactgegevenFromKbo;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class WijzigContactgegevenRequest
{
    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public TeWijzigenContactgegeven Contactgegeven { get; set; } = null!;

    public WijzigContactgegevenFromKboCommand ToCommand(string vCode, int contactgegevenId)
        => new(
            VCode.Create(vCode),
            new WijzigContactgegevenFromKboCommand.CommandContactgegeven(
                contactgegevenId,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
