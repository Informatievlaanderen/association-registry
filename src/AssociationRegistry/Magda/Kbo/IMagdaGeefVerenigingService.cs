namespace AssociationRegistry.Magda.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using ResultNet;

public interface IMagdaGeefVerenigingService
{
    Task<Result> GeefVereniging(
        KboNummer kboNummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);
}

public interface IMagdaSyncGeefVerenigingService
{
    Task<Result> GeefVereniging(KboNummer kboNummer, AanroependeFunctie aanroependeFunctie, CommandMetadata metadata, CancellationToken cancellationToken);
}
