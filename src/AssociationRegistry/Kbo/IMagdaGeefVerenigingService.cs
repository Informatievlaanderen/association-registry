namespace AssociationRegistry.Kbo;

using ResultNet;
using Vereniging;

public interface IMagdaGeefVerenigingService
{
    Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, string initiator, CancellationToken cancellationToken);
}
