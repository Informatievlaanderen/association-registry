namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Schema;
using Schema.Search;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        services.AddSingleton<ElasticClient>(serviceProvider
                                                 => CreateElasticClient(elasticSearchOptions,
                                                                        serviceProvider.GetRequiredService<ILogger<ElasticClient>>()));

        services.AddSingleton<IElasticClient>(serviceProvider
                                                  => CreateElasticClient(elasticSearchOptions,
                                                                         serviceProvider.GetRequiredService<ILogger<ElasticClient>>()));

        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IElasticClient>();
            var mapping = client.Indices.GetMapping<VerenigingZoekDocument>();

            return mapping.Indices[elasticSearchOptions.Indices!.Verenigingen!].Mappings;
        });

        return services;
    }

    public static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        var settings = new ConnectionSettings(
                           new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .ServerCertificateValidationCallback((o, certificate, arg3, arg4) =>
                       {
                           logger.LogWarning("Policy errors: [{Cert}|{Chain}] {Error}", certificate.Subject, arg3, arg4.ToString());
                           logger.LogWarning("Policy object: {@Error}", o);

                           if (arg4 != SslPolicyErrors.None)
                           {
                               logger.LogWarning(Convert.ToBase64String(certificate.Export(X509ContentType.Cert),
                                                                        Base64FormattingOptions.InsertLineBreaks));

                               LogCertificateDetails(certificate, arg3);
                           }

                           return true;
                       })
                      .IncludeServerStackTraceOnError()
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        if (elasticSearchOptions.EnableDevelopmentLogs)
            settings = settings.DisableDirectStreaming()
                               .PrettyJson()
                               .OnRequestCompleted(apiCallDetails =>
                                {
                                    if (apiCallDetails.RequestBodyInBytes != null)
                                        logger.LogDebug(
                                            message: "ES Request: {HttpMethod} {Uri} \n {RequestBody}",
                                            apiCallDetails.HttpMethod,
                                            apiCallDetails.Uri,
                                            Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));

                                    if (apiCallDetails.ResponseBodyInBytes != null)
                                        logger.LogDebug(message: "ES Response: {ResponseBody}",
                                                        Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes));

                                    if (apiCallDetails.DebugInformation != null)
                                        logger.LogDebug(message: "ES Debug: {ResponseBody}",
                                                        apiCallDetails.DebugInformation);
                                });

        return new ElasticClient(settings);
    }

    private static void LogCertificateDetails(X509Certificate certificate, X509Chain chain)
    {
        if (certificate != null)
        {
            Console.WriteLine("Certificate Subject: " + certificate.Subject);
            Console.WriteLine("Certificate Issuer: " + certificate.Issuer);
        }

        if (chain != null)
        {
            foreach (var chainElement in chain.ChainElements)
            {
                Console.WriteLine("Certificate Subject: " + chainElement.Certificate.Subject);
                Console.WriteLine("Certificate Issuer: " + chainElement.Certificate.Issuer);
            }
        }
    }
}
