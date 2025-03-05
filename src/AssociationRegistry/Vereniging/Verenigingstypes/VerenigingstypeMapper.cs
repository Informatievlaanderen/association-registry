namespace AssociationRegistry.Vereniging;

public interface IVerenigingstypeMapper
{
    TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingstype, new()
        where TSource : IVerenigingstype, new();
}

public class VerenigingstypeMapperV1 : IVerenigingstypeMapper
{
    public TDestination Map<TDestination, TSource>(TSource verenigingsType)
    where TDestination : IVerenigingstype, new()
    where TSource : IVerenigingstype, new()
    {
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
}

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
}
