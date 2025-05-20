namespace AssociationRegistry.Test.Projections.Beheer.SanityChecks;

using Admin.ProjectionHost.Projections.PowerBiExport;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using Events;
using Marten.Events;
using System.Reflection;
using Xunit;

public class For_Any_Event
{
    // Because we want to update the metadata for each event!
    [Fact]
    public void There_Should_Be_A_Create_Or_Project_Method()
    {
        var eventTypes = typeof(Events.IEvent).Assembly
                                                                     .GetTypes()
                                                                     .Where(t => typeof(Events.IEvent)
                                                                               .IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                                                                     .Except([typeof(AfdelingWerdGeregistreerd), typeof(GeotagsWerdenBepaald)]) // because it's obsolete
                                                                     .ToList();

        var projectionType = typeof(BeheerVerenigingDetailProjection);
        var methods = projectionType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var eventType in eventTypes)
        {
            var hasCreateOrProjectMethod = methods.Any(m =>
                                                           (m.Name == "Create" || m.Name == "Project") &&
                                                           m.GetParameters().Any(p =>
                                                                                     p.ParameterType.IsGenericType &&
                                                                                     p.ParameterType.GetGenericTypeDefinition() == typeof(IEvent<>) &&
                                                                                     p.ParameterType.GetGenericArguments()[0] == eventType
                                                           )
            );

            Assert.True(hasCreateOrProjectMethod, $"No Create or Project method found for event type {eventType.Name}");
        }
    }

    [Fact]
    public void There_Should_Be_A_Create_Or_Project_Method_For_PowerBi()
    {
        var eventTypes = typeof(Events.IEvent).Assembly
                                                                     .GetTypes()
                                                                     .Where(t => typeof(Events.IEvent)
                                                                               .IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                                                                     .Except([typeof(AfdelingWerdGeregistreerd), typeof(GeotagsWerdenBepaald)]) // because it's obsolete
                                                                     .ToList();

        var projectionType = typeof(PowerBiExportProjection);
        var methods = projectionType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var eventType in eventTypes)
        {
            var hasCreateOrProjectMethod = methods.Any(m =>
                                                           (m.Name == "Create" || m.Name == "Project" || m.Name == "Apply") &&
                                                           m.GetParameters().Any(p =>
                                                                                     p.ParameterType.IsGenericType &&
                                                                                     p.ParameterType.GetGenericTypeDefinition() == typeof(IEvent<>) &&
                                                                                     p.ParameterType.GetGenericArguments()[0] == eventType
                                                           )
            );

            Assert.True(hasCreateOrProjectMethod, $"No Create or Project or Apply method found for event type {eventType.Name}");
        }
    }
}
