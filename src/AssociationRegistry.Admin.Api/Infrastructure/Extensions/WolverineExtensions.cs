namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Amazon.Runtime;
using EventStore;
using Grar.AddressMatch;
using JasperFx.CodeGeneration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Vereniging;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;

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

                options.OnException<UnexpectedAggregateVersionDuringSyncException>().RetryWithCooldown(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                );

                var addressMatchOptionsSection = context.Configuration.GetAddressMatchOptionsSection();
                Log.Logger.Information("Address match configuration: {@Config}", addressMatchOptionsSection);

                if (addressMatchOptionsSection.OptimizeArtifactWorkflow)
                {
                    options.OptimizeArtifactWorkflow(TypeLoadMode.Static);
                }

                var transportConfiguration = options.UseAmazonSqsTransport(config =>
                {
                    Log.Logger.Information("Wolverine SQS configuration: {@Config}", config);
                    config.ServiceURL = addressMatchOptionsSection.SqsTransportServiceUrl;
                });

                if (addressMatchOptionsSection.UseLocalStack)
                {
                    transportConfiguration.Credentials(new BasicAWSCredentials("dummy", "dummy"));
                }

                options.PublishMessage<TeSynchroniserenAdresMessage>()
                       .ToSqsQueue(addressMatchOptionsSection.AddressMatchSqsQueueName);

                options.ListenToSqsQueue(addressMatchOptionsSection.AddressMatchSqsQueueName, configure =>
                        {
                            configure.DeadLetterQueueName = addressMatchOptionsSection.AddressMatchSqsDeadLetterQueueName;
                        })
                       .ConfigureDeadLetterQueue(addressMatchOptionsSection.AddressMatchSqsDeadLetterQueueName, queue =>
                        {
                            queue.DeadLetterQueueName = addressMatchOptionsSection.AddressMatchSqsDeadLetterQueueName;
                        })
                       .MaximumParallelMessages(1);

                if (addressMatchOptionsSection.AutoProvision)
                {
                    transportConfiguration.AutoProvision();
                }

                options.LogMessageStarting(LogLevel.Trace);
                Log.Logger.Information("Wolverine Transport SQS configuration: {@TransportConfig}", transportConfiguration);
            });
    }
}
