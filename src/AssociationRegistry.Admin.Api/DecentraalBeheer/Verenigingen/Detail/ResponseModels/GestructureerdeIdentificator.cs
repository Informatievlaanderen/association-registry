﻿namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class GestructureerdeIdentificator
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    /// De externe identificator van de vereniging in de bron
    /// </summary>
    [DataMember(Name = "Nummer")]
    public string Nummer { get; set; } = null!;
}
