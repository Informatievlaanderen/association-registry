namespace AssociationRegistry.Kbo;

using ResultNet;
using Vereniging;

public interface IMagdaGeefVerenigingService
{
    Task<Result> GeefVereniging(KboNummer kboNummer, string initiator, CancellationToken cancellationToken);
}
