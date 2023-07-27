namespace AssociationRegistry.Kbo;

using ResultNet;

public interface IMagdaGeefVerenigingService
{
    Task<Result> GeefVereniging(string kboNummer);
}
