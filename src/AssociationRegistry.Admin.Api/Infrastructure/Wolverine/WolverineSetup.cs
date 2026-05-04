namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine;

using global::Wolverine;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using global::Wolverine.ErrorHandling;
using global::Wolverine.Marten;
using Integrations.Magda.Shared.Exceptions;
using JasperFx;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using Marten;
using MartenDb.Setup;
using Pipelines;
using ProjectionHost.Projections.Bewaartermijn.EventHandling;
using Queues;
using Serilog;

public static class WolverineSetup
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        const string wolverineSchema = "public";

        builder.Host.UseWolverine(
            (options) =>
            {
                Log.Logger.Information("Setting up wolverine");
                options.ApplicationAssembly = typeof(Program).Assembly;
                options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);
                options.Discovery.IncludeAssembly(
                    typeof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler).Assembly
                );

                options
                    .OnException<UnexpectedAggregateVersionDuringSyncException>()
                    .RetryWithCooldown(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5)
                    );

                RegistreerVzerPipeline.Setup(options);
                RegistreerErkenningPipeline.Setup(options);

                VoegVertegenwoordigerToePipeline.Setup(options);

                SqsWolverineSetup.ConfigureSqsQueues(options, builder.Configuration);
                PostgresWolverineSetup.ConfigureQueues(options, wolverineSchema, builder.Configuration);

                options.AutoBuildMessageStorageOnStartup = AutoCreate.All;
                options.UseNewtonsoftForSerialization(settings => settings.ConfigureForVerenigingsregister());
            }
        );
    }
}
