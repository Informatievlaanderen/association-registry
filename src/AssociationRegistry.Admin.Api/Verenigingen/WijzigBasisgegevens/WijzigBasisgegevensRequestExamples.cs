namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System;
using CommonRequestDataTypes;
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
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2023, 1, 1)),
            ContactInfoLijst = new[]
            {
                new ContactInfo
                {
                    Contactnaam = "Naam van het contact",
                    SocialMedia = "https://example.org",
                    Website = "https://example.org",
                    PrimairContactInfo = false,
                    Telefoon = "123456789",
                    Email = "info@example.org",
                },
            },
            Initiator = "OVO000001",
        };
}
