namespace AssociationRegistry.Test.Admin.Api.Persoonsgegevens;

using Events;
using FluentAssertions;
using MartenDb.Store;
using Xunit;

public class PersoonsgegevensTransformerCoverageTests
{
    [Fact]
    public void Every_ZonderPersoonsgegevens_event_has_a_transformer()
    {
        var eventAssembly = typeof(BankrekeningnummerWerdToegevoegdZonderPersoonsgegevens).Assembly;
        var transformerAssembly = typeof(IPersoonsgegevensEventTransformer).Assembly;

        var persistedEventTypes =
            eventAssembly.GetTypes()
                         .Where(t => t.IsClass && !t.IsAbstract
                                                  &&                     typeof(IEvent).IsAssignableFrom(t)
                                               && t.Name.EndsWith("ZonderPersoonsgegevens", StringComparison.Ordinal))
                         .ToArray();

        persistedEventTypes.Should().NotBeEmpty(
            "if this is empty you are scanning the wrong assembly for events");

        var transformers =
            transformerAssembly.GetTypes()
                               .Where(t => typeof(IPersoonsgegevensEventTransformer).IsAssignableFrom(t)
                                        && t.IsClass && !t.IsAbstract)
                               .Select(t => (IPersoonsgegevensEventTransformer)Activator.CreateInstance(t)!)
                               .ToArray();

        foreach (var persisted in persistedEventTypes)
        {
            transformers.Any(tr => tr.PersistedEventType == persisted)
                        .Should().BeTrue($"{persisted.FullName} must be produced by a transformer");
        }
    }
}

