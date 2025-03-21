﻿namespace AssociationRegistry.Vereniging;

public interface IVerenigingstypeMapper
{
    TDestination Map<TDestination, TSource>(TSource verenigingsType)
        where TDestination : IVerenigingstype, new()
        where TSource : IVerenigingstype, new();

    TDestination? MapSubtype<TDestination, TSource>(TSource? subtype)
        where TDestination : IVerenigingssubtype, new()
        where TSource : IVerenigingssubtype, new();

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map)
        where TSource : IVerenigingssubtype, new();

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

    public TDestination? MapSubtype<TDestination, TSource>(TSource? subtype) where TDestination : IVerenigingssubtype, new() where TSource : IVerenigingssubtype, new()
        => default;

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map) where TSource : IVerenigingssubtype, new()
        => default;
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

    public TDestination? MapSubtype<TDestination, TSource>(TSource? subtype) where TDestination : IVerenigingssubtype, new() where TSource : IVerenigingssubtype, new()
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

    public TDestination? MapSubverenigingVan<TDestination, TSource>(TSource? subtype, Func<TDestination> map) where TSource : IVerenigingssubtype, new()
    {
        if (subtype is null || subtype.Code != Verenigingssubtype.Subvereniging.Code)
        {
            return default;
        }
        return map();
    }
}
