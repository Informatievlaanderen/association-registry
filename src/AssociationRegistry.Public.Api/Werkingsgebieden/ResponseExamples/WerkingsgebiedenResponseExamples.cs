﻿namespace AssociationRegistry.Public.Api.Werkingsgebieden.ResponseExamples;

using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class WerkingsgebiedenResponseExamples : IExamplesProvider<WerkingsgebiedenResponse[]>
{
    public WerkingsgebiedenResponse[] GetExamples()
        =>
        [
            new()
            {
                Werkingsgebieden =
                    AssociationRegistry.Vereniging.Werkingsgebied.AllWithNVTExamples
                                                              .Take(3)
                                                              .Select(wg => new Werkingsgebied
                                                               {
                                                                   Code = wg.Code,
                                                                   Naam = wg.Naam,
                                                               })
                                                              .ToArray()
                                    .Union(AssociationRegistry.Vereniging.Werkingsgebied.AllExamples
                                                              .Skip(7)
                                                              .Take(3)
                                                              .Select(wg => new Werkingsgebied
                                                               {
                                                                   Code = wg.Code,
                                                                   Naam = wg.Naam,
                                                               }))
                                    .ToArray(),
            },
        ];
}
