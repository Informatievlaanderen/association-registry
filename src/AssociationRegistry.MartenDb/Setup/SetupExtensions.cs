namespace AssociationRegistry.MartenDb.Setup;

using Admin.Api.Adapters.VCodeGeneration;
using DecentraalBeheer.Vereniging;
using Marten;
using Upcasters;
using Events;
using Formats;
using Hosts.Configuration.ConfigurationBindings;
using Marten.Services;
using Newtonsoft.Json;
using Serialization;
using Vereniging;

public static class SetupExtensions
{
    public static void UpcastLegacyTombstoneEvents(this StoreOptions source)
    {
        source.Events.Upcast(
            new TombstoneUpcaster()
        );
    }

    public static StoreOptions RegisterAllEventTypes(this StoreOptions opts)
    {
        var eventInterface = typeof(Events.IEvent);

        var eventTypes = eventInterface.Assembly
                                       .GetTypes()
                                       .Where(t =>
                                                  eventInterface.IsAssignableFrom(t) &&
                                                  t.IsClass &&
                                                  !t.IsAbstract &&
                                                  t != typeof(AfdelingWerdGeregistreerd) // skip obsolete one if needed
                                        );

        foreach (var eventType in eventTypes)
        {
            opts.Events.AddEventType(eventType);
        }

        return opts;
    }

    public static void ConfigureForVerenigingsregister(this JsonSerializerSettings settings)
    {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
    }

    public static StoreOptions SetUpOpenTelemetry(this StoreOptions opts)
    {
        opts.OpenTelemetry.TrackConnections = TrackLevel.Normal;
        opts.OpenTelemetry.TrackEventCounters();

        return opts;
    }

    public static StoreOptions UsePostgreSqlOptions(this StoreOptions opts, PostgreSqlOptionsSection postgreSqlOptions)
    {
        opts.Connection(postgreSqlOptions.GetConnectionString());

        if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
        {
            opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
            opts.DatabaseSchemaName = postgreSqlOptions.Schema;
        }

        return opts;
    }

    public static StoreOptions AddVCodeSequence(this StoreOptions opts)
    {
        opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));

        return opts;
    }

    public static StoreOptions ConfigureSerialization(this StoreOptions opts)
    {
        opts.UseNewtonsoftForSerialization(configure: settings => settings.ConfigureForVerenigingsregister());

        return opts;
    }
}
