namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Microsoft.OpenApi.Models;

public static class ConfigureSwaggerExtensions
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection source)
    {
        source.AddEndpointsApiExplorer();
        source.AddSwaggerGen(options =>
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Basisregisters Vlaanderen Verenigingsregister Publieke Projecties API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Digitaal Vlaanderen",
                        Email = "digitaal.vlaanderen@vlaanderen.be",
                        Url = new Uri("https://publiek.verenigingen.vlaanderen.be"),
                    },
                })
        );
        return source;
    }
}
