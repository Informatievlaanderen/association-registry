namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning.Examples;

using CorrigeerSchorsingErkenning.RequestModels;
using Primitives;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class CorrigeerErkenningRequestExamples : IExamplesProvider<CorrigeerErkenningRequest>
{
    public CorrigeerErkenningRequest GetExamples() =>
        new()
        {
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 1,1)),
            Einddatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 12,31)),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 10,31)),
            HernieuwingsUrl = "https://www.website.com/renew",
        };
}
