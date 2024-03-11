namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class PropertyDescriptorExtensions
{
    public static TDescriptor WithKeyword<TDescriptor, TInterface, T>(
        this CorePropertyDescriptorBase<TDescriptor, TInterface, T> source,
        string? normalizer = null)
        where TDescriptor : CorePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
        where TInterface : class, ICoreProperty
        where T : class
    {
        return source.Fields(x =>
                                 x.Keyword(
                                     y =>
                                         normalizer is null
                                             ? y.Name("keyword")
                                             : y.Name("keyword").Normalizer(normalizer)));
    }
}
