namespace AssociationRegistry.Test.Projections.SanityChecks;

using System.Reflection;
using Admin.ProjectionHost.Projections.Detail;
using Admin.ProjectionHost.Projections.PowerBiExport;
using Events;
using JasperFx.Events;
using MartenDb.Transformers;
using Public.ProjectionHost.Projections.Sequence;
using IEvent = Events.IEvent;

public class For_Any_Event
{
    private IEnumerable<Type> _excludedEventTypes =
    [
        typeof(BewaartermijnWerdVerlopen),
        typeof(BewaartermijnWerdGestart),
        typeof(AfdelingWerdGeregistreerd),
        typeof(BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis),
        typeof(BankrekeningnummerWerdGevalideerd),
        typeof(KboNummerWerdGereserveerd),
        typeof(DubbeleVerenigingenWerdenGedetecteerd),
        typeof(BewaartermijnWerdGestartV2), // TODO undo for powerbi
        typeof(ErkenningWerdGecorrigeerd),
        typeof(ErkenningOpvolgersWerdenToegevoegdAlsBeheerder), // TODO check with analist if this needs to be exposed or not
        .. new PersoonsgegevensEventTransformers().Select(x => x.PersistedEventType),
    ];

    // Because we want to update the metadata for each event!
    [Fact]
    public void There_Should_Be_A_Create_Or_Project_Method()
    {
        var eventTypes = typeof(IEvent)
            .Assembly.GetTypes()
            .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .Except(_excludedEventTypes) // only add events that are obsolete
            .Except([typeof(VerenigingWerdErkend), typeof(VerenigingWerdNietLangerErkend)])
            .ToList();

        var projectionType = typeof(BeheerVerenigingDetailProjection);
        var methods = projectionType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var eventType in eventTypes)
        {
            var hasCreateOrProjectMethod = methods.Any(m =>
                (m.Name == "Create" || m.Name == "Project")
                && m.GetParameters()
                    .Any(p =>
                        p.ParameterType.IsGenericType
                        && p.ParameterType.GetGenericTypeDefinition() == typeof(IEvent<>)
                        && p.ParameterType.GetGenericArguments()[0] == eventType
                    )
            );

            Assert.True(hasCreateOrProjectMethod, $"No Create or Project method found for event type {eventType.Name}");
        }
    }

    [Fact]
    public void There_Should_Be_A_Create_Or_Project_Method_For_PowerBi()
    {
        var eventTypes = typeof(IEvent)
            .Assembly.GetTypes()
            .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .Except(_excludedEventTypes) // only add events that are obsolete
            .Except([typeof(ErkenningWerdGeregistreerd)]) // TODO implement in other PR
            .ToList();

        var projectionType = typeof(PowerBiExportProjection);
        var methods = projectionType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var eventType in eventTypes)
        {
            var hasCreateOrProjectMethod = methods.Any(m =>
                (m.Name == "Create" || m.Name == "Project" || m.Name == "Apply")
                && m.GetParameters()
                    .Any(p =>
                        p.ParameterType.IsGenericType
                        && p.ParameterType.GetGenericTypeDefinition() == typeof(IEvent<>)
                        && p.ParameterType.GetGenericArguments()[0] == eventType
                    )
            );

            Assert.True(
                hasCreateOrProjectMethod,
                $"No Create or Project or Apply method found for event type {eventType.Name}"
            );
        }
    }

    [Fact]
    public void There_Should_Be_A_Create_Or_Project_Method_For_Sequence_Projection()
    {
        var eventTypes = typeof(IEvent)
            .Assembly.GetTypes()
            .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .Except(_excludedEventTypes) // only add events that are obsolete
            .ToList();

        var projectionType = typeof(PubliekVerenigingSequenceProjection);
        var methods = projectionType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var eventType in eventTypes)
        {
            var hasCreateOrProjectMethod = methods.Any(m =>
                (m.Name == "Create" || m.Name == "Project" || m.Name == "Apply")
                && m.GetParameters()
                    .Any(p =>
                        p.ParameterType.IsGenericType
                        && p.ParameterType.GetGenericTypeDefinition() == typeof(IEvent<>)
                        && p.ParameterType.GetGenericArguments()[0] == eventType
                    )
            );

            Assert.True(
                hasCreateOrProjectMethod,
                $"No Create or Project or Apply method found for event type {eventType.Name}"
            );
        }
    }
}
