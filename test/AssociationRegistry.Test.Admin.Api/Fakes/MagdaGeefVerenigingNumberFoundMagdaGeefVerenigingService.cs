namespace AssociationRegistry.Test.Admin.Api.Fakes;

using AssociationRegistry.Framework;
using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(VerenigingVolgensKbo verenigingVolgensKbo)
    {
        _verenigingVolgensKbo = verenigingVolgensKbo;
    }

    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeldigeVereniging(_verenigingVolgensKbo));
}
