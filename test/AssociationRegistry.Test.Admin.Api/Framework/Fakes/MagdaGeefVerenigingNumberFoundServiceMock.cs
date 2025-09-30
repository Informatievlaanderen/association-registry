namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundServiceMock : IMagdaGeefVerenigingService
{

    private readonly Result _result;



    public MagdaGeefVerenigingNumberFoundServiceMock(VerenigingVolgensKbo verenigingVolgensKbo)
    {
            _result = VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo);
    }

    public MagdaGeefVerenigingNumberFoundServiceMock(InactieveVereniging inactieve)
    {
            _result = VerenigingVolgensKboResult.InactieveVereniging(inactieve);
    }

    public Task<Result> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(_result);

    public Task<Result> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
        => Task.FromResult(_result);
}

public class MagdaSyncGeefVerenigingNumberFoundServiceMock : IMagdaSyncGeefVerenigingService
{

    private readonly Result _result;



    public MagdaSyncGeefVerenigingNumberFoundServiceMock(VerenigingVolgensKbo verenigingVolgensKbo)
    {
            _result = VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo);
    }

    public MagdaSyncGeefVerenigingNumberFoundServiceMock(InactieveVereniging inactieve)
    {
            _result = VerenigingVolgensKboResult.InactieveVereniging(inactieve);
    }

    public Task<Result> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(_result);

    public Task<Result> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
        => Task.FromResult(_result);
}
