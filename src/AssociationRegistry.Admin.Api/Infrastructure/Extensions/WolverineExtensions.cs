namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Grar.AddressMatch;
using JasperFx.CodeGeneration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Vereniging;
using Wolverine;
using Wolverine.AmazonSqs;

public static class WolverineExtensions
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        builder.Host.UseWolverine(
            (context, options) =>
            {
                Log.Logger.Error("Use wolverine");

                options.ApplicationAssembly = typeof(Program).Assembly;
                options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);
                options.Discovery.IncludeType<TeSynchroniserenAdresMessage>();
                options.Discovery.IncludeType<TeSynchroniserenAdresMessageHandler>();

                options.OptimizeArtifactWorkflow(TypeLoadMode.Static);

                var addressMatchOptionsSection = context.Configuration.GetAddressMatchOptionsSection();

                //options.UseNewtonsoftForSerialization(conf => ConfigureJsonSerializerSettings());

                if (addressMatchOptionsSection.UseLocalStack)
                {
                    options.UseAmazonSqsTransport(config =>
                            {
                                config.ServiceURL = "http://127.0.0.1:4566";
                            })
                           .AutoProvision()
                           .Credentials(new BasicAWSCredentials("dummy", "dummy"));
                }
                else
                {
                    options.UseAmazonSqsTransport(config =>
                    {
                        config.ServiceURL = addressMatchOptionsSection.SqsTransportServiceUrl;
                    })
                   .AutoProvision();
                }

                options.PublishMessage<TeSynchroniserenAdresMessage>()
                       .ToSqsQueue(addressMatchOptionsSection.AddressMatchSqsQueueName)
                       .SendInline();

                options.ListenToSqsQueue(addressMatchOptionsSection.AddressMatchSqsQueueName)
                       .ProcessInline();

                options.LogMessageStarting(LogLevel.Information);
            });
    }
}
