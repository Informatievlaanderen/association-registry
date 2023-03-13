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
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2023,1,1)),
            Initiator = "OVO000001",
        };
}
