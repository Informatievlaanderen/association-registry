namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

public static class DevelopmentExtensions
{
    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;
        return app
            .UseDeveloperExceptionPage()
            .UseMigrationsEndPoint()
            .UseBrowserLink();
    }
}
