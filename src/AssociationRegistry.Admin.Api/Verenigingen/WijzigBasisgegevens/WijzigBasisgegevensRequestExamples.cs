namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System;
using Primitives;
using Swashbuckle.AspNetCore.Filters;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new()
        {
            Naam = "Naam van de vereniging",
            KorteNaam = "Korte naam van de vereniging",
            KorteBeschrijving = "Korte beschrijving van de vereniging",
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(year: 2023, month: 1, day: 1)),
            Initiator = "OVO000001",
        };
}
