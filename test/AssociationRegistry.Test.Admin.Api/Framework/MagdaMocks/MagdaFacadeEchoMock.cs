namespace AssociationRegistry.Test.Admin.Api.Framework.MagdaMocks;

using Magda;
using Vereniging;

public class MagdaFacadeEchoMock: IMagdaFacade
{
    public Task<MagdaPersoon> GetByInsz(Insz insz, CancellationToken cancellationToken = default)
        => Task.FromResult(new MagdaPersoon
        {
            IsOverleden = false,
            Insz = insz,
            Achternaam = insz,
            Voornaam = insz,
        });
}
