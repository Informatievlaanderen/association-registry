namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerErkenningRequestExamples : IExamplesProvider<RegistreerErkenningRequest>
{
    public RegistreerErkenningRequest GetExamples() =>
        new()
        {
            Erkenning = new()
            {
                IpdcProductNummer = "38738",
                Startdatum = new DateOnly(2026, 1, 1),
                Einddatum = new DateOnly(2026, 12, 31),
                Hernieuwingsdatum = new DateOnly(2026, 10, 31),
                HernieuwingsUrl = "https://www.website.com/renew"
            },
        };
}
