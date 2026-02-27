// See https://aka.ms/new-console-template for more information
using AssociationRegistry.Admin.ProjectionHost.Projections.Search.Zoeken;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.MartenDb;
using AssociationRegistry.MartenDb.Logging;
using AssociationRegistry.MartenDb.Setup;
using JasperFx;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Wolverine;
using Wolverine.Marten;
using Wolverine.Postgresql;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) =>
        {
            var configuration = context.Configuration;

            // however you load your PostgreSqlOptionsSection
            var postgreSqlOptions = configuration.GetSection("PostgreSQLOptions").Get<PostgreSqlOptionsSection>()!;

            // Call your extension method
            services
                .AddMarten(serviceProvider =>
                {
                    var opts = new StoreOptions();

                    opts.UsePostgreSqlOptions(postgreSqlOptions).AddVCodeSequence().ConfigureSerialization();

                    opts.Logger(
                        new SecureMartenLogger(serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>())
                    );

                    opts.Events.StreamIdentity = StreamIdentity.AsString;
                    opts.Events.MetadataConfig.EnableAll();
                    opts.Events.AppendMode = EventAppendMode.Quick;

                    return opts;
                })
                .IntegrateWithWolverine(integration =>
                {
                    integration.TransportSchemaName = WellknownSchemaNames.Wolverine;
                    integration.MessageStorageSchemaName = WellknownSchemaNames.Wolverine;
                });

            services.AddWolverine(options =>
            {
                Log.Logger.Information("Setting up wolverine");

                const string AanvaardDubbeleVerenigingQueueName = "aanvaard-dubbele-vereniging-queue";

                options
                    .PublishMessage<AanvaardDubbeleVerenigingMessage>()
                    .ToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);

                options.ListenToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);
            });
        }
    );

using var host = builder.Build();

await host.RunJasperFxCommands(args);
