namespace AssociationRegistry.Integrations.Magda;

using AssociationRegistry.Magda.Kbo;
using Framework;
using Onderneming.Models;
using Onderneming.Models.GeefOnderneming;
using Onderneming.Models.RegistreerInschrijving;
using Persoon.Models;
using Persoon.Models.RegistreerUitschrijving;
using Shared.Models;

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

    Task<ResponseEnvelope<Persoon.Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>?>
        RegistreerInschrijvingPersoon(
            string insz,
            AanroependeFunctie aanroependeFunctie,
            CommandMetadata metadata,
            CancellationToken cancellationToken);
}
