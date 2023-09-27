﻿namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using Schema.VerenigingenPerInsz;
using System.Linq;

public static class VerenigingPerInszMapper
{
    public static VerenigingenPerInszResponse ToResponse(this VerenigingenPerInszDocument doc)
        => new()
        {
            Insz = doc.Insz,
            Verenigingen = doc.Verenigingen.Select(Map).ToArray(),
        };

    private static VerenigingenPerInszResponse.Vereniging Map(Vereniging v)
        => new()
        {
            Naam = v.Naam,
            Status = v.Status,
            KboNummer = v.KboNummer,
            VCode = v.VCode,
        };
}
