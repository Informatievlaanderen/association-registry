namespace AssociationRegistry.DecentraalBeheer.Vereniging.Mappers;

public class VerenigingstypeMapperV2 : IVerenigingstypeMapper
{
   public TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingstype, new()
        where TSource : IVerenigingstype, new()
    {
        if (Verenigingstype.IsGeenKboVereniging(verenigingsType.Code))
        {
            return new TDestination
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            };
        }

        return new TDestination()
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
        };
    }

    public TDestination? MapSubtype<TDestination, TSource>(TSource? subtype) where TDestination : IVerenigingssubtypeCode, new() where TSource : IVerenigingssubtypeCode
    {
        if (subtype is null)
        {
            return default;
        }

        return new TDestination
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };
    }

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map) where TSource : IVerenigingssubtypeCode, new()
    {
        if (subtype is null || subtype.Code != VerenigingssubtypeCode.Subvereniging.Code)
        {
            return default;
        }
        return map();
    }
}
