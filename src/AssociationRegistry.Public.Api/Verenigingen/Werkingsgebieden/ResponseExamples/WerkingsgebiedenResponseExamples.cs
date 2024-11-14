namespace AssociationRegistry.Public.Api.Verenigingen.Werkingsgebieden.ResponseExamples;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class WerkingsgebiedenResponseExamples : IExamplesProvider<WerkingsgebiedenResponse>
{
    private readonly AppSettings _appSettings;

    public WerkingsgebiedenResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public WerkingsgebiedenResponse GetExamples()
        => new()
        {
            Werkingsgebieden = [],
        };
}
