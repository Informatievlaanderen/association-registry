namespace AssociationRegistry.Kbo;

using DecentraalBeheer.Vereniging;
using Framework;
using ResultNet;
using Vereniging;

public interface IMagdaRegistreerInschrijvingService
{
    Task<Result> RegistreerInschrijving(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
}
