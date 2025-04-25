namespace AssociationRegistry.Vereniging;

public static class VerenigingssubtypeExtensions
{
    public static TDestination Convert<TDestination>(this IHasVerenigingssubtypeCodeAndNaam subtype) where TDestination : IHasVerenigingssubtypeCodeAndNaam, new()
        => new()
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };
}
