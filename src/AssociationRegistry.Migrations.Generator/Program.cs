using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Migrations.Generator;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

class Program
{
    static async Task Main(string[] args)
    {
        // Typical console host setup
        using var host = Host.CreateDefaultBuilder(args)
                             .UseWolverine(WolverineBootstrap.Configure)
                             .ConfigureServices((context, services) =>
                              {
                                  // Call your extension method
                                  services.AddMartenV2();

                                  // register any other services you need
                              })
                             .Build();

        await host.RunJasperFxCommands(args);
    }
}
