namespace AssociationRegistry.Test.Admin.Api.Fakes;

using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, string initiator, CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeldigeVereniging(new VerenigingVolgensKbo { KboNummer = kboNummer }));
}
