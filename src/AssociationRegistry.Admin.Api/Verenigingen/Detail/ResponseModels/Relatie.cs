﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Relatie
{
    /// <summary>
    ///     Het type relatie
    /// </summary>
    [DataMember(Name = "Relatietype")]
    public string Relatietype { get; set; } = null!;

    /// <summary>
    ///     de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;
}
