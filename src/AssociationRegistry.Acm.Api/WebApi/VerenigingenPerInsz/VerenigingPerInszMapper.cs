namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AssociationRegistry.Vereniging.Mappers;
using System.Linq;
using Vereniging = Schema.VerenigingenPerInsz.Vereniging;

public static class VerenigingPerInszMapper
{
    public static VerenigingenPerInszResponse ToResponse(VerenigingenPerInszDocument doc, VerenigingenPerKbo[] verenigingenPerKbo)
        => new()
        {
            Insz = doc.Insz,
            Verenigingen = doc.Verenigingen.Where(x => !x.IsDubbel).Select(Map).ToArray(),
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
            Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(vereniging.Verenigingstype.Code, vereniging.Verenigingstype.Naam),
            Verenigingssubtype =
                new VerenigingstypeMapperV2()
                   .MapSubtype<VerenigingenPerInszResponse.Verenigingssubtype,
                        AssociationRegistry.Acm.Schema.VerenigingenPerInsz.Verenigingssubtype>(vereniging.Verenigingssubtype),
            IsHoofdvertegenwoordigerVan = vereniging.IsHoofdvertegenwoordigerVan,
        };

    private static VerenigingenPerInszResponse.VerenigingenPerKbo Map(VerenigingenPerKbo verenigingenPerKbo)
        => new()
        {
            KboNummer = verenigingenPerKbo.KboNummer,
            VCode = verenigingenPerKbo.VCode,
            IsHoofdVertegenwoordiger = verenigingenPerKbo.IsHoofdvertegenwoordiger,
        };
}
