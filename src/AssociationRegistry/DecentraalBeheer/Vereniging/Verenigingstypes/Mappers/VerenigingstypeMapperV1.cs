namespace AssociationRegistry.DecentraalBeheer.Vereniging.Mappers;

public class VerenigingstypeMapperV1 : IVerenigingstypeMapper
{
    public TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingstype, new()
        where TSource : IVerenigingstype, new()
    {
        if (verenigingsType is null)
            throw new NullReferenceException(nameof(verenigingsType));

        if (Verenigingstype.IsGeenKboVereniging(verenigingsType.Code))
        {
            return new TDestination()
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            };
        }

        return new TDestination()
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
        };
    }

    public TDestination? MapSubtype<TDestination, TSource>(TSource? subtype) where TDestination : IVerenigingssubtypeCode, new() where TSource : IVerenigingssubtypeCode
        => default;

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map) where TSource : IVerenigingssubtypeCode, new()
        => default;
}
