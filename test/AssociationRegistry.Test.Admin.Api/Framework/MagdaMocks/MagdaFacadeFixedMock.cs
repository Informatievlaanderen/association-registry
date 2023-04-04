namespace AssociationRegistry.Test.Admin.Api.Framework.MagdaMocks;

using INSZ;
using Magda;

public class MagdaFacadeFixedMock: IMagdaFacade
{
    private readonly MagdaPersoon _persoonToReturn;

    public MagdaFacadeFixedMock(MagdaPersoon persoonToReturn)
    {
        _persoonToReturn = persoonToReturn;
    }

    public Task<MagdaPersoon> GetByInsz(Insz insz, CancellationToken cancellationToken = default)
        => Task.FromResult(_persoonToReturn);
}
