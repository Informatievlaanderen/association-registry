namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplication;

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
