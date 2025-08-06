namespace AssociationRegistry.DecentraalBeheer.Vereniging.Mappers;

public static class VerenigingssubtypeMapperExtensions
{
    public static TDestination Map<TDestination>(this IVerenigingssubtypeCode subtype) where TDestination : IVerenigingssubtypeCode, new()
        => new()
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };
}
