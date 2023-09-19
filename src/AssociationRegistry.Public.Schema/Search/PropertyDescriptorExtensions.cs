namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class PropertyDescriptorExtensions
{
    public static TDescriptor WithKeyword<TDescriptor, TInterface, T>(this CorePropertyDescriptorBase<TDescriptor, TInterface, T> source)
        where TDescriptor : CorePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
        where TInterface : class, ICoreProperty
        where T : class
        => source.Fields(x => x.Keyword(y => y.Name("keyword")));
}
