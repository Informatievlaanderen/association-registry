﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;
using Vereniging;

/// <summary>
/// Het subtype van de vereniging
/// </summary>
public class Verenigingssubtype : IVerenigingssubtype
{
    /// <summary>
    /// De code van het subtype
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het subtype
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
