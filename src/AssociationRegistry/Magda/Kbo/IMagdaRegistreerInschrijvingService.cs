namespace AssociationRegistry.Magda.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using ResultNet;

public interface IMagdaRegistreerInschrijvingService
{
    Task<Result> RegistreerInschrijving(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
}
