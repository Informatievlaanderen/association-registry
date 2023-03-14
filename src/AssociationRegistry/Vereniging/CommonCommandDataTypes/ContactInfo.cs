namespace AssociationRegistry.Vereniging.CommonCommandDataTypes;

public record ContactInfo(
    string Contactnaam,
    string? Email,
    string? Telefoon,
    string? Website,
    string? SocialMedia,
    bool PrimairContactInfo);
