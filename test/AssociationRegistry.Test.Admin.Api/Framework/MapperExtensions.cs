namespace AssociationRegistry.Test.Admin.Api.Framework;

public static class MapperExtensions
{
    public static Vereniging.CommonCommandDataTypes.ContactInfo ToCommandDataType(this Events.CommonEventDataTypes.ContactInfo source)
        => new(source.Contactnaam, source.Email, source.Telefoon, source.Website, source.SocialMedia, source.PrimairContactInfo);

    public static AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes.ContactInfo ToRequestDataType(this Events.CommonEventDataTypes.ContactInfo source)
        => new()
        {
            Contactnaam = source.Contactnaam,
            Email = source.Email,
            Telefoon = source.Telefoon,
            Website = source.Website,
            SocialMedia = source.SocialMedia,
            PrimairContactInfo = source.PrimairContactInfo,
        };

    public static Events.CommonEventDataTypes.ContactInfo ToEventDataType(this Vereniging.CommonCommandDataTypes.ContactInfo source)
        => new(source.Contactnaam, source.Email, source.Telefoon, source.Website, source.SocialMedia, source.PrimairContactInfo);
}
