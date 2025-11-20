namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Store;
using System.Collections.ObjectModel;

public class PersoonsgegevensEventTransformers : ReadOnlyCollection<IPersoonsgegevensEventTransformer>
{
    public IEnumerable<Type> PersistedEventTypes => this.Select(x => x.PersistedEventType);
    public PersoonsgegevensEventTransformers() : base(CollectTransformers())
    {

    }

    private static IList<IPersoonsgegevensEventTransformer> CollectTransformers()
    {
        var interfaceType = typeof(IPersoonsgegevensEventTransformer);

        var implementations = typeof(VertegenwoordigerWerdToegevoegdTransformer).Assembly.GetTypes()
                                                                                .Where(t =>
                                                                                           interfaceType.IsAssignableFrom(t) &&
                                                                                           t is { IsInterface: false, IsAbstract: false })
                                                                                .Select(t => (IPersoonsgegevensEventTransformer)Activator.CreateInstance(t)!)
                                                                                .ToList();

        return implementations;
    }
}

public class VertegenwoordigerWerdToegevoegdTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdToegevoegd);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (VertegenwoordigerWerdToegevoegd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(
            refId,
            original.VertegenwoordigerId,
            original.IsPrimair
        );

        var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.VertegenwoordigerId,
            Insz.Hydrate(original.Insz),
            original.Roepnaam,
            original.Rol,
            original.Voornaam,
            original.Achternaam,
            original.Email,
            original.Telefoon,
            original.Mobiel,
            original.SocialMedia
        );

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}

// public class VertegenwoordigerWerdVerwijderdTransformer : IPersoonsgegevensEventTransformer
// {
//     public Type EventType => typeof(VertegenwoordigerWerdVerwijderd);
//
//     public Task<EventTransformationResult> Transform(IEvent @event, VCode vCode)
//     {
//         var original = (VertegenwoordigerWerdVerwijderd)@event;
//
//         // Assuming you have a zonder persoonsgegevens version
//         var transformedEvent = new VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(
//             original.VertegenwoordigerId,
//             original.RefId // Assuming it has the RefId to the persoonsgegevens
//         );
//
//         // No new persoonsgegevens to save when removing
//         return Task.FromResult(new EventTransformationResult(transformedEvent, null));
//     }
// }
//
// public class VertegenwoordigerWerdGewijzigdTransformer : IPersoonsgegevensEventTransformer
// {
//     public Type EventType => typeof(VertegenwoordigerWerdGewijzigd);
//
//     public Task<EventTransformationResult> Transform(IEvent @event, VCode vCode)
//     {
//         var original = (VertegenwoordigerWerdGewijzigd)@event;
//         var refId = Guid.NewGuid();
//
//         var transformedEvent = new VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(
//             refId,
//             original.VertegenwoordigerId,
//             original.IsPrimair
//         );
//
//         var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
//             refId,
//             vCode,
//             original.VertegenwoordigerId,
//             Insz.Hydrate(original.Insz),
//             original.Roepnaam,
//             original.Rol,
//             original.Voornaam,
//             original.Achternaam,
//             original.Email,
//             original.Telefoon,
//             original.Mobiel,
//             original.SocialMedia
//         );
//
//         return Task.FromResult(new EventTransformationResult(transformedEvent, persoonsgegevens));
//     }
// }
