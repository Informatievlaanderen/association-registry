namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Mappers;
using Swashbuckle.AspNetCore.Filters;
using VerenigingStatus = Schema.Constants.VerenigingStatus;

public class VerenigingenPerInszResponseExamples : IExamplesProvider<VerenigingenPerInszResponse>
{
    public VerenigingenPerInszResponse GetExamples()
        => new()
        {
            Insz = "12345678901",
            Verenigingen = new[]
            {
                new VerenigingenPerInszResponse.Vereniging
                {
                    VCode = "V1234567",
                    Naam = "FWA De vrolijke BA’s",
                    Status = VerenigingStatus.Actief,
                    CorresponderendeVCodes = [],
                    IsHoofdvertegenwoordigerVan = false,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(Verenigingstype.VZER.Code,
                                                                                      Verenigingstype.VZER.Naam),
                    Verenigingssubtype = VerenigingssubtypeCode.NietBepaald.Map<VerenigingenPerInszResponse.Verenigingssubtype>(),
                    KboNummer = "",
                    VertegenwoordigerId = 0,
                },
                new VerenigingenPerInszResponse.Vereniging
                {
                    VCode = "V7654321",
                    Naam = "FWA De Bron",
                    Status = VerenigingStatus.Gestopt,
                    CorresponderendeVCodes = [],
                    IsHoofdvertegenwoordigerVan = false,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(Verenigingstype.VZW.Code,
                                                                                      Verenigingstype.VZW.Naam),
                    KboNummer = "",
                    VertegenwoordigerId = 0,
                },
            },
            KboNummers =
            [
                new VerenigingenPerInszResponse.VerenigingenPerKbo
                {
                    KboNummer = "1234567890",
                    IsHoofdVertegenwoordiger = true,
                    VCode = "V0995511",
                },
                new VerenigingenPerInszResponse.VerenigingenPerKbo
                {
                    KboNummer = "0987654321",
                    IsHoofdVertegenwoordiger = false,
                    VCode = VerenigingenPerKbo.VCodeUitzonderingen.NietVanToepassing,
                },
            ],
        };
}
