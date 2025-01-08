namespace AssociationRegistry.Grar.Models.PostalInfo;

public record PostalInformationResponse(
    string Postcode,
    string Gemeentenaam,
    Postnamen Postnamen);
