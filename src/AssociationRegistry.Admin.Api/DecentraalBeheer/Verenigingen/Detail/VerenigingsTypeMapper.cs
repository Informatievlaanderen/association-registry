namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using Schema;
using Vereniging;

public interface IVerenigingsTypeMapper
{
    TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingsType, new()
        where TSource : IVerenigingsType, new();
}

public class VerenigingsTypeMapper : IVerenigingsTypeMapper
{
    public TDestination Map<TDestination, TSource>(TSource verenigingsType)
    where TDestination : IVerenigingsType, new()
    where TSource : IVerenigingsType, new()
    {
        if (Verenigingstype.IsGeenKboVereniging(verenigingsType.Code))
        {
            return new TDestination
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            };
        }

        return new TDestination()
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam
        };
    }
}

public class VerenigingsTypeMapperV2 : IVerenigingsTypeMapper
{
   public TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingsType, new()
        where TSource : IVerenigingsType, new()
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
