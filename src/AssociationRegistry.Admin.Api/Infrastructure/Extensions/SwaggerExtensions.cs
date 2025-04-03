namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Asp.Versioning.ApiExplorer;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using Documentation;
using Hosts;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Primitives;
using Swagger;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

public static class SwaggerExtensions
{
    public static IServiceCollection AddAdminApiSwagger(this IServiceCollection services, AppSettings appSettings)
        => services
          .AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly())
          .AddSwaggerGen(
               options =>
               {
                   options.AddXmlComments(Assembly.GetExecutingAssembly().GetName().Name!);
                   options.DescribeAllParametersInCamelCase();
                   options.UseAllOfToExtendReferenceSchemas();
                   options.SupportNonNullableReferenceTypes();
                   options.SchemaFilter<DisableNullableFilter>();
                   options.SchemaFilter<ExampleSchemaFilter>();
                   options.OperationFilter<CapitalizeParameterFilter>();

                   options.MapType<DateOnly>(
                       () => new OpenApiSchema
                       {
                           Type = "string",
                           Format = "date",
                           Pattern = "yyyy-MM-dd",
                       });

                   options.MapType<NullOrEmpty<DateOnly>>(
                       () => new OpenApiSchema
                       {
                           Type = "string",
                           Nullable = true,
                       });

                   options.CustomSchemaIds(type => type.FullName);

                   options.SwaggerDoc(
                       "v1",
                       new OpenApiInfo
                       {
                           Version = "v1",
                           Extensions = new Dictionary<string, IOpenApiExtension>
                           {
                               {
                                   WellknownOpenApiExtensions.XAssemblyversion, new Microsoft.OpenApi.Any.OpenApiString(Assembly.GetAssembly(typeof(Program)).GetVersionText())
                               },
                           },
                           Title = appSettings.ApiDocs.Title,
                           Description = @$"
Voor meer algemene informatie over het gebruik van deze API, raadpleeg onze [publieke confluence pagina](https://vlaamseoverheid.atlassian.net/wiki/spaces/AGB/pages/6285361348/API+documentatie).

## Gebruik API Versies
Om gebruik te kunnen maken van een andere API versie, is het noodzakelijk een API versie mee te geven.

Deze dienen meegestuurd te worden als header, of via de query parameters.

Om gebruik te maken van de v1, geef je geen api versie mee.

Je kan bepalen of een endpoint meerdere versies ondersteunt, door de aanwezigheid van de request header `vr-api-key` bij de documentatie van dat endpoint.

Mogelijke waarden zijn:
* {WellknownVersions.V2} – in deze versie wordt het verenigingstype omgezet van `FV - Feitelijke vereniging` naar `VZER - Vereniging zonder eigen rechtspersoonlijkheid`.

| Type            | Naam             | Voorbeeld                                                                                                                                                       |
|-----------------|------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Header          | `vr-api-version` | `curl --request GET --url '{appSettings.BaseUrl}/v1/verenigingen' --header 'vr-api-version: {WellknownVersions.V2}'`                      |
| Query parameter | `vr-api-version` | {appSettings.BaseUrl}/v1/verenigingen?vr-api-version={WellknownVersions.V2}                                                                   |

### Wijzigingen in v2

Sinds de release van v2 werden bestaande verenigingen van het type FV-feitelijke vereniging omgezet naar VZER-vereniging zonder eigen rechtspersoonlijkheid.
De verenigingen krijgen door deze migratie 2 extra velden, namelijk `subverenigingVan` en `verenigingssubtype`.

Verenigingen zonder eigen rechtspersoonlijkheid die na deze migratie aangemaakt zijn, zullen automatisch dit nieuwe type toegewezen krijgen,
en krijgen als verenigingssubtype standaard de waarde `Niet bepaald`.

Bij het opvragen van deze verenigingen zonder de v2-header, worden echter nog steeds de velden en semantiek van v1 gehanteerd.
",
                           Contact = new OpenApiContact
                           {
                               Name = appSettings.ApiDocs.Contact.Name,
                               Email = appSettings.ApiDocs.Contact.Email,
                               Url = new Uri(appSettings.ApiDocs.Contact.Url),
                           },
                       });

                   options.ExampleFilters();

                   options.SchemaFilter<AutoRestSchemaFilter>();

                   options.OperationFilter<SwaggerDefaultValues>();
                   options.OperationFilter<DescriptionOperationFilter>();
                   options.OperationFilter<AddResponseHeadersFilter>();
                   options.OperationFilter<TagByApiExplorerSettingsOperationFilter>();
                   options.OperationFilter<AuthorizationResponseOperationFilter>();
                   options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                   options.OperationFilter<AppendCorrelationIdToHeaders>();
                   options.OperationFilter<AppendInitiatorToHeaders>();

                   options.OrderActionsBy(a => $"{a.RelativePath}.{a.HttpMethod}");

                   options.DocInclusionPredicate((_, _) => true);
               })
          .AddSwaggerGenNewtonsoftSupport();

    public static IApplicationBuilder ConfigureAdminApiSwagger(this IApplicationBuilder app)
        => app.UseSwaggerDocumentation(
            new SwaggerDocumentationOptions
            {
                ApiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>(),
                DocumentTitleFunc = groupName => $"Basisregisters Vlaanderen - Verenigingsregister Beheer API {groupName}",
                HeadContentFunc = _ => Documentation.GetHeadContent(),
                FooterVersion = Assembly.GetExecutingAssembly().GetVersionText(),
                CSharpClient =
                {
                    ClassName = "Verenigingsregister",
                    Namespace = "Be.Vlaanderen.Basisregisters",
                },
                TypeScriptClient =
                {
                    ClassName = "Verenigingsregister",
                },
            });

    private static void AddXmlComments(this SwaggerGenOptions swaggerGenOptions, string name)
    {
        var possiblePaths = new[]
        {
            CreateXmlCommentsPath(AppContext.BaseDirectory, name),
            CreateXmlCommentsPath(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, name),
            CreateXmlCommentsPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, name),
        };

        foreach (var possiblePath in possiblePaths)
        {
            if (!File.Exists(possiblePath))
                continue;

            swaggerGenOptions.IncludeXmlComments(possiblePath);

            return;
        }

        throw new ApplicationException(
            $"Could not find swagger xml docs. Locations where I searched:\n\t- {string.Join(separator: "\n\t-", possiblePaths)}");
    }

    private static string CreateXmlCommentsPath(string directory, string name)
        => Path.Combine(directory, $"{name}.xml");
}
