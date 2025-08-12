namespace AssociationRegistry.Grar.NutsLau;

public interface IPostcodesFromGrarFetcher
{
    Task<string[]> FetchPostalCodes(CancellationToken cancellationToken);
}
