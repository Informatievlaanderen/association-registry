namespace AssociationRegistry.Grar.NutsLau;

public interface INutsLauFromGrarFetcher
{
    Task<PostalNutsLauInfo[]> GetFlemishAndBrusselsNutsAndLauByPostcode(CancellationToken cancellationToken);
}
