namespace AssociationRegistry.Public.Api.WebApi.Werkingsgebieden.ResponseExamples;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class WerkingsgebiedenResponseExamples : IExamplesProvider<WerkingsgebiedenResponse[]>
{
    public WerkingsgebiedenResponse[] GetExamples()
        =>
        [
            new()
            {
                Werkingsgebieden =
                    WellKnownWerkingsgebieden.Provincies
                                             .Take(3)
                                             .Select(wg => new Werkingsgebied
                                              {
                                                  Code = wg.Code,
                                                  Naam = wg.Naam,
                                              })
                                             .ToArray()
                                             .Concat([
                                                  new Werkingsgebied
                                                  {
                                                      Code = "BE2411540", Naam = "Pajottegem",
                                                  },
                                                  new Werkingsgebied
                                                  {
                                                      Code = "BE2122223", Naam = "Heist-op-den-Berg",
                                                  },
                                              ])
                                             .ToArray(),
            },
        ];
}
