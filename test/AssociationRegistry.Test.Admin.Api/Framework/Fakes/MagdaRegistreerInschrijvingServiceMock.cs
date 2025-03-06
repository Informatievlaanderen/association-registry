namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Framework;
using Kbo;
using ResultNet;
using System.Threading;
using System.Threading.Tasks;
using Vereniging;

public class MagdaRegistreerInschrijvingServiceMock : IMagdaRegistreerInschrijvingService
{
    private readonly Result _mockResult;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public MagdaRegistreerInschrijvingServiceMock(Result mockResult)
    {
        _mockResult = mockResult;
    }

    public async Task<Result> RegistreerInschrijving(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
        => _mockResult;
}
