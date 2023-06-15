﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; } = null!;
}