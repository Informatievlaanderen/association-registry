namespace AssociationRegistry.Integrations.Magda;

using AssociationRegistry.Magda.Kbo;
using Framework;
using GeefOnderneming.Models;
using Models;
using Models.GeefPersoon;
using Models.RegistreerInschrijving;
using Models.RegistreerUitschrijving;

public interface IMagdaClient
{
    Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(
        string kbonummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);

    Task<ResponseEnvelope<RegistreerInschrijvingResponseBody>?> RegistreerInschrijvingOnderneming(
        string kbonummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);

    Task<ResponseEnvelope<GeefPersoonResponseBody>?> GeefPersoon(
        string insz,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);

    Task<ResponseEnvelope<RegistreerUitschrijvingResponseBody>?> RegistreerUitschrijving(
        string kbonummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken);

    Task<ResponseEnvelope<Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>?>
        RegistreerInschrijvingPersoon(
            string insz,
            AanroependeFunctie aanroependeFunctie,
            CommandMetadata metadata,
            CancellationToken cancellationToken);
}
