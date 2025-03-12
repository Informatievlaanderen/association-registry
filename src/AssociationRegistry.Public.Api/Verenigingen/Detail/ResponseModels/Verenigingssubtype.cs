﻿namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

/// <summary>
/// Het subtype van de vereniging
/// </summary>
public class Verenigingssubtype : IVerenigingssubtype
{
    /// <summary>
    /// De code van het subtype vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het subtype vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}

