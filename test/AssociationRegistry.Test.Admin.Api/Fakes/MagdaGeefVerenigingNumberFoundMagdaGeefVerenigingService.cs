namespace AssociationRegistry.Test.Admin.Api.Fakes;

using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly VerenigingVolgensKbo? _verenigingVolgensKbo;

    public MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(VerenigingVolgensKbo? verenigingVolgensKbo = null)
    {
        _verenigingVolgensKbo = verenigingVolgensKbo;
    }

    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeldigeVereniging(_verenigingVolgensKbo ?? VerenigingVolgensKbo(kboNummer)));

    private static VerenigingVolgensKbo VerenigingVolgensKbo(KboNummer kboNummer)
    {
        var v = new Fixture().CustomizeAdminApi().Create<VerenigingVolgensKbo>() with
        {
            KboNummer = kboNummer,
            Type = Verenigingstype.VZW,
        };

        return v;
    }
}
