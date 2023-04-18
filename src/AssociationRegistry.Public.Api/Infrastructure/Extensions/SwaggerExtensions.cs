namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

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
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerExtensions
{
    public static IServiceCollection AddPublicApiSwagger(this IServiceCollection services, AppSettings appSettings)
        => services
            .AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly())
            .AddSwaggerGen(
                options =>
                {
                    options.AddXmlComments(Assembly.GetExecutingAssembly().GetName().Name!);
                    options.DescribeAllParametersInCamelCase();
                    options.SupportNonNullableReferenceTypes();
                    options.MapType<DateOnly>(
                        () => new OpenApiSchema
                        {
                            Type = "string",
                            Format = "date",
                            Pattern = "yyyy-MM-dd",
                        });
                    options.CustomSchemaIds(type => type.FullName);
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Basisregisters Vlaanderen Verenigingsregister Publieke API",
                            Description = Documentation.Documentation.GetApiLeadingText(appSettings),
                            Contact = new OpenApiContact
                            {
                                Name = "Digitaal Vlaanderen",
                                Email = "digitaal.vlaanderen@vlaanderen.be",
                                Url = new Uri("https://beheer.verenigingen.vlaanderen.be"),
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
                    options.OrderActionsBy(SortByTag.Sort);

                    options.DocInclusionPredicate((_, _) => true);
                })
            .AddSwaggerGenNewtonsoftSupport();

    public static IApplicationBuilder ConfigurePublicApiSwagger(this IApplicationBuilder app)
        => app.UseSwaggerDocumentation(
            new SwaggerDocumentationOptions
            {
                ApiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>(),
                DocumentTitleFunc = groupName => $"Basisregisters Vlaanderen - Verenigingsregister Publieke API {groupName}",
                FooterVersion = Assembly.GetExecutingAssembly().GetVersionText(),
                HeadContentFunc = _ => $@"
                             <style>
                                 div:has(> h1)
                                 {{
                                     padding-bottom: 0px !important;
                                 }}
                                 div:has(> h2)
                                 {{
                                     padding-top: 0px !important;
                                     padding-bottom: 0px !important;
                                 }}
                                 h3
                                 {{
                                     margin-top: 2em !important;
                                 }}
                                 li[data-item-id=""section/Toegang-tot-het-register""]
                                 {{
                                     border-bottom: 1px solid rgb(225, 225, 225);
                                 }}
                             </style>",

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
