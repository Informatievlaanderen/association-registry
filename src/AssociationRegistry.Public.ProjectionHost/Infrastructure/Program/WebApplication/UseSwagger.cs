namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplication;

using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

public static class UseSwaggerExtensions
{
    public static void SetUpSwagger(this WebApplication source)
    {
        source.UseSwagger();
        source.UseSwaggerUI();

        source.UseReDoc(
            options =>
            {
                options.RoutePrefix = "docs";
                options.DocumentTitle = "Swagger Demo Documentation";
            });
    }
}
