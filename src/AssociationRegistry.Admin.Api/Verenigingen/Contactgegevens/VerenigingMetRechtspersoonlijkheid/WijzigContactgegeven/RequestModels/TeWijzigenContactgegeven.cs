﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;

using System.Runtime.Serialization;

/// <summary>Het te wijzigen contactgegeven</summary>
[DataContract]
public class TeWijzigenContactgegeven
{
    /// <summary>
    /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "beschrijving")] public string? Beschrijving { get; set; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "isPrimair")]
    public bool? IsPrimair { get; set; }
}
