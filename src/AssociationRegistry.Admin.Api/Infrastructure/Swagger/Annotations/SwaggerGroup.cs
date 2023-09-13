namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;

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

    public class VerrijkingenVanKboAttribute : ApiExplorerSettingsAttribute
    {
        public VerrijkingenVanKboAttribute()
        {
            GroupName = "Verrijkingen van gegevens uit KBO";
        }
    }

    public class OpvragenAttribute : ApiExplorerSettingsAttribute
    {
        public OpvragenAttribute()
        {
            GroupName = "Opvragen van verenigingen";
        }
    }
    public class ContextenAttribute : ApiExplorerSettingsAttribute
    {
        public ContextenAttribute()
        {
            GroupName = "Contexten";
        }
    }

    public class ParametersAttribute : ApiExplorerSettingsAttribute
    {
        public ParametersAttribute()
        {
            GroupName = "Parameters";
        }
    }
}
