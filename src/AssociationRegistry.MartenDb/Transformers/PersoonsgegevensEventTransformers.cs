namespace AssociationRegistry.MartenDb.Transformers;

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
