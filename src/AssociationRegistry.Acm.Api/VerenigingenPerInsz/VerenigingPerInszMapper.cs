namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using Queries.VerenigingenPerKboNummer;
using Schema.VerenigingenPerInsz;
using System.Linq;

public static class VerenigingPerInszMapper
{
    public static VerenigingenPerInszResponse ToResponse(VerenigingenPerInszDocument doc, VerenigingenPerKbo[] verenigingenPerKbo)
        => new()
        {
            Insz = doc.Insz,
            Verenigingen = doc.Verenigingen.Select(Map).ToArray(),
            KboNummers = verenigingenPerKbo.Select(Map).ToArray(),
        };

    private static VerenigingenPerInszResponse.Vereniging Map(Vereniging vereniging)
        => new()
        {
            Naam = vereniging.Naam,
            CorresponderendeVCodes = vereniging.CorresponderendeVCodes,
            VertegenwoordigerId = vereniging.VertegenwoordigerId,
            Status = vereniging.Status,
            KboNummer = vereniging.KboNummer,
            VCode = vereniging.VCode,
            Verenigingstype = new Verenigingstype(vereniging.Verenigingstype.Code, vereniging.Verenigingstype.Naam),
            IsHoofdvertegenwoordigerVan = vereniging.IsHoofdvertegenwoordigerVan,
        };

    private static VerenigingenPerInszResponse.VerenigingenPerKbo Map(VerenigingenPerKbo verenigingenPerKbo)
        => new()
        {
            KboNummer = verenigingenPerKbo.KboNummer,
            VCode = verenigingenPerKbo.VCode,
            IsHoofdVertegenwoordiger = verenigingenPerKbo.IsHoofdvertegenwoordiger
        };
}
