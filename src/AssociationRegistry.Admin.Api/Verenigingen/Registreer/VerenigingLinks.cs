﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Runtime.Serialization;

/// <summary>Weblinks i.v.m. deze vereniging</summary>
[DataContract]
public class VerenigingLinks
{
    public VerenigingLinks(Uri detail)
    {
        Detail = detail;
    }

    /// <summary>De link naar het beheer detail van de vereniging</summary>
    [DataMember(Name = "Detail")]
    public Uri Detail { get; init; }
}