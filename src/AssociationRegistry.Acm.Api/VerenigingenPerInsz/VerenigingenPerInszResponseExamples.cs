namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

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
                    { VCode = "V1234567", Naam = "FWA De vrolijke BA’s", Status = VerenigingStatus.Actief },
                new VerenigingenPerInszResponse.Vereniging { VCode = "V7654321", Naam = "FWA De Bron", Status = VerenigingStatus.Gestopt },
                new VerenigingenPerInszResponse.Vereniging
                    { VCode = "V0995511", Naam = "VZW De Kost", Status = VerenigingStatus.Actief, KboNummer = "1234567890" },
            },
        };
}
