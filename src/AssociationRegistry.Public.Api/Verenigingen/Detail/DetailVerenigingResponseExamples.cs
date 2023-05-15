namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System;
using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    public DetailVerenigingResponse GetExamples()
        => new(
            "",
            new VerenigingDetail(
                "V0001001",
                "FWA De vrolijke BA’s",
                "DVB",
                "De vereniging van de vrolijke BA's",
                new DateOnly(2020, 05, 15),
                "Actief",
                new[]
                {
                    new Contactgegeven("Email", "info@example.org", "Info", false),
                },
                new[]
                {
                    new Locatie(
                        "Correspondentie",
                        true,
                        "kerkstraat 5, 1770 Liedekerke, Belgie",
                        "de kerk",
                        "kerkstraat",
                        "5",
                        null,
                        "1770",
                        "Liedekerke",
                        "Belgie"),
                },
                new[]
                {
                    new HoofdactiviteitVerenigingsloket("CULT", "Cultuur"),
                }
            ),
            new Metadata("2023-05-15"));
}
