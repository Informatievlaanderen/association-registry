using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using JasperFx.Events;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.CommandLine;

var inszOption = new Option<string>(
    name: "--insz",
    description: "The INSZ (Rijksregisternummer) to register with Magda")
{
    IsRequired = true
};

var rootCommand = new RootCommand("Magda Test Runner - Register INSZ with Magda")
{
    inszOption
};

rootCommand.SetHandler(async (insz) =>
{
    // Set up Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();

    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddSerilog(Log.Logger);
    });

    var logger = loggerFactory.CreateLogger<MagdaClient>();

    Log.Information("Registering INSZ: {Insz}", insz);

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>()
        .Build();

    var connectionString = Environment.GetEnvironmentVariable("ConnectionString")
                        ?? "Host=localhost;Port=5432;Database=verenigingsregister;Username=root;Password=root";

    Log.Information("Connecting to database: {ConnectionString}", connectionString.Replace("Password=root", "Password=***"));

    var store = DocumentStore.For(options =>
    {
        options.Connection(connectionString);
        options.Events.StreamIdentity = StreamIdentity.AsString;
        options.Events.MetadataConfig.EnableAll();
        options.Events.AppendMode = EventAppendMode.Quick;
    });

    var mySettings = config.GetSection("MagdaOptions").Get<MagdaOptionsSection>();

    if (mySettings == null)
    {
        Log.Error("MagdaOptions not found in configuration");
        return;
    }

    await using var lightweightSession = store.LightweightSession();

    var client = new MagdaClient(
        mySettings,
        new MagdaCallReferenceService(new MagdaCallReferenceRepository(lightweightSession)),
        logger
    );

    try
    {
        var magdaCallReference = new MagdaCallReference()
        {
            Reference = Guid.NewGuid(),
        };

        var registreerInschrijvingPersoon = await client.RegistreerInschrijvingPersoon(insz, magdaCallReference);

        var persoon = await client.GeefPersoon(insz, CommandMetadata.ForDigitaalVlaanderenProcess, CancellationToken.None);

        Log.Information(JsonConvert.SerializeObject(persoon));
        Log.Information("Successfully registered INSZ with Magda");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to register INSZ");
        throw;
    }
    finally
    {
        await Log.CloseAndFlushAsync();
    }
}, inszOption);

return await rootCommand.InvokeAsync(args);
