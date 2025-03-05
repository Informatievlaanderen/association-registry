namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using AcmBevraging;
using Schema.Constants;
using Swashbuckle.AspNetCore.Filters;

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
                    Verenigingstype = new Verenigingstype(Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                                                          Vereniging.Verenigingstype.FeitelijkeVereniging.Naam),
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
                    Verenigingstype = new Verenigingstype(Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                                                          Vereniging.Verenigingstype.FeitelijkeVereniging.Naam),
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
