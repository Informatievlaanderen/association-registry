namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.Examples;

using Primitives;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigErkenningRequestExamples : IExamplesProvider<WijzigErkenningRequest>
{
    public WijzigErkenningRequest GetExamples() =>
        new()
        {
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 1,1)),
            Einddatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 12,31)),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 10,31)),
            HernieuwingsUrl = "https://www.website.com/renew",
            RedenVanWijziging = "Lorem ipsum dolores sit amen",
        };
}
