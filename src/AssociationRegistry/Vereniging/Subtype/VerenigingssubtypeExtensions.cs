namespace AssociationRegistry.Vereniging;

public static class VerenigingssubtypeExtensions
{
    public static TDestination Convert<TDestination>(this IVerenigingssubtypeCode subtype) where TDestination : IVerenigingssubtypeCode, new()
        => new()
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };
}
