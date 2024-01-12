namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Microsoft.AspNetCore.Localization;
using System.Globalization;

public static class ConfigureRequestLocalizationExtensions
{
    public static IServiceCollection ConfigureRequestLocalization(this IServiceCollection source)
    {
        source.Configure<RequestLocalizationOptions>(
            opts =>
            {
                const string fallbackCulture = "en-GB";
                var defaultRequestCulture = new RequestCulture(new CultureInfo(fallbackCulture));
                var supportedCulturesOrDefault = new[] { new CultureInfo(fallbackCulture) };

                opts.DefaultRequestCulture = defaultRequestCulture;
                opts.SupportedCultures = supportedCulturesOrDefault;
                opts.SupportedUICultures = supportedCulturesOrDefault;

                opts.FallBackToParentCultures = true;
                opts.FallBackToParentUICultures = true;
            });

        return source;
    }
}
