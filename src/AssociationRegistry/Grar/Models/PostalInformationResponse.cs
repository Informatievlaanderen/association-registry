namespace AssociationRegistry.Grar.Models;

public record PostalInformationResponse(
    string Postcode,
    string Gemeentenaam,
    string[] Postnamen);
