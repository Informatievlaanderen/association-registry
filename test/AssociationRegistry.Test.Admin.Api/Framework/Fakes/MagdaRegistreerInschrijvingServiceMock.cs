namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using ResultNet;
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
