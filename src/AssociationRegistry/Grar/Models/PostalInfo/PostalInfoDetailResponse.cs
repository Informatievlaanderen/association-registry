namespace AssociationRegistry.Grar.Models.PostalInfo;

public record PostalInfoDetailResponse(
    string Postcode,
    string Gemeentenaam,
    Postnamen Postnamen);
