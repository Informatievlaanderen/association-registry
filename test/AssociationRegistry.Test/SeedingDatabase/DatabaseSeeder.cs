namespace AssociationRegistry.Test.SeedingDatabase;

using Admin.Api.Infrastructure.Json;
using Admin.ProjectionHost.Constants;
using AssociationRegistry.Framework;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Events;
using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Exceptions;
using Marten.Services;
using Newtonsoft.Json;
using NodaTime.Text;

public class DatabaseSeeder
{
    //[Fact(Skip = "Enable when needed")]
    //[Fact]
    public async Task SeedData()
{
    var fixture = new Fixture().CustomizeAdminApi();
    var context = new SpecimenContext(fixture);

    var documentStore = await CreateAsync("public");
    await using var session = documentStore.LightweightSession();

    var eventTypes = new[]
    {
        typeof(LocatieWerdToegevoegd),
        typeof(ContactgegevenWerdToegevoegd),
        typeof(LidmaatschapWerdToegevoegd),
        typeof(VertegenwoordigerWerdToegevoegd),
        typeof(WerkingsgebiedenWerdenBepaald),
    };

    var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
    session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
    session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
    session.CorrelationId = metadata.CorrelationId.ToString();

    var streamKeys = new List<string>();

    // Step 1: Create 500 geregistreerd events (one per stream)
    for (int i = 0; i < 5000; i++)
    {
        while (true)
        {
            var registratieEvent = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
            var streamKey = registratieEvent.VCode;

            try
            {
                session.Events.StartStream(streamKey, registratieEvent);
                await session.SaveChangesAsync();

                streamKeys.Add(streamKey); // keep track of all registered streams
                break;
            }
            catch (ExistingStreamIdCollisionException)
            {
                continue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    // Step 2: For each stream, append the other events
    foreach (var streamKey in streamKeys)
    {
        foreach (var _ in Enumerable.Range(0, eventTypes.Length))
        {
            var eventType = eventTypes[new Random().Next(eventTypes.Length)];
            var eventInstance = context.Resolve(eventType);
            session.Events.Append(streamKey, eventInstance);
        }
        await session.SaveChangesAsync();
    }
}


    public static async Task<DocumentStore> CreateAsync(string schema)
    {
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection("host=127.0.0.1:5432;" +
                               "database=verenigingsregister;" +
                               "password=root;" +
                               "username=root");

            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Events.MetadataConfig.EnableAll();
            options.Serializer(CreateCustomMartenSerializer());
            options.UseNewtonsoftForSerialization(configure: settings =>
            {
                settings.DateParseHandling = DateParseHandling.None;
                settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        });

        return documentStore;
    }

    public static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Configure(
            s =>
            {
            });

        return jsonNetSerializer;
    }
}
