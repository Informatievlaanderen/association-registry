namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Microsoft.AspNetCore.Mvc;

public static class SwaggerGroup
{
    public class RegistratieAttribute : ApiExplorerSettingsAttribute
    {
        public RegistratieAttribute()
        {
            GroupName = "Registratie";
        }
    }

    public class DecentraalBeheerAttribute : ApiExplorerSettingsAttribute
    {
        public DecentraalBeheerAttribute()
        {
            GroupName = "Decentraal beheer van verenigingen";
        }
    }

    public class WijzigenVanKboAttribute : ApiExplorerSettingsAttribute
    {
        public WijzigenVanKboAttribute()
        {
            GroupName = "Wijzigen van gegevens uit KBO";
        }
    }

    public class OpvragenAttribute : ApiExplorerSettingsAttribute
    {
        public OpvragenAttribute()
        {
            GroupName = "Opvragen van verenigingen";
        }
    }
}
