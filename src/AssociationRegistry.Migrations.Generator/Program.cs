using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Migrations.Generator;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        // Typical console host setup
        using var host = Host.CreateDefaultBuilder(args)
                             .ConfigureServices((context, services) =>
                              {
                                  var configuration = context.Configuration;

                                  // however you load your PostgreSqlOptionsSection
                                  var postgresOptions = configuration
                                                       .GetSection("PostgreSql")
                                                       .Get<PostgreSqlOptionsSection>()!;

                                  // Call your extension method
                                  services.AddMarten(configuration, postgresOptions, isDevelopment: true);

                                  // register any other services you need
                              })
                             .Build();

        await host.RunJasperFxCommands(args);
        await host.StartAsync();
    }
}
