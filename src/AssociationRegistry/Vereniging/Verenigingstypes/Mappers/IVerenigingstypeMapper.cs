namespace AssociationRegistry.Vereniging.Mappers;

public interface IVerenigingstypeMapper
{
    TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingstype, new()
        where TSource : IVerenigingstype, new();

    TDestination? MapSubtype<TDestination, TSource>(TSource? subtype)
        where TDestination : IVerenigingssubtypeCode, new()
        where TSource : IVerenigingssubtypeCode;

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map)
        where TSource : IVerenigingssubtypeCode, new();

}
