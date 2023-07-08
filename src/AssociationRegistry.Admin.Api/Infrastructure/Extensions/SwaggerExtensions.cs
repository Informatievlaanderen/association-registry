namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;
using System.IO;
using System.Reflection;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using ConfigurationBindings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Primitives;
using Swagger;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                    options.SupportNonNullableReferenceTypes();
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
                            Format = "date",
                            Pattern = "yyyy-MM-dd",
                            Nullable = true,
                        });
                    options.CustomSchemaIds(type => type.FullName);
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = appSettings.ApiDocs.Title,
                            Description = "---\n" +
                                          "Voor meer algemene informatie over het gebruik van deze API, raadpleeg onze " +
                                          "<a href=\"https://vlaamseoverheid.atlassian.net/wiki/spaces/AGB/pages/6285361348/API+documentatie\">publieke confluence pagina</a>.",
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
                HeadContentFunc = _ => Documentation.Documentation.GetHeadContent(),
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
            $"Could not find swagger xml docs. Locations where I searched:\n\t- {string.Join("\n\t-", possiblePaths)}");
    }

    private static string CreateXmlCommentsPath(string directory, string name)
        => Path.Combine(directory, $"{name}.xml");
}
