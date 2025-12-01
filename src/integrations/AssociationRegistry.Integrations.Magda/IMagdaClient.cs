namespace AssociationRegistry.Integrations.Magda;

using Framework;
using GeefOnderneming.Models;
using Models;
using Models.GeefOndernemingVKBO;
using Models.RegistreerInschrijving;
using Models.RegistreerUitschrijving;

public interface IMagdaClient
{
    Task<ResponseEnvelope<GeefOndernemingVKBOResponseBody>?> GeefOndernemingVKBO(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<ResponseEnvelope<RegistreerInschrijvingResponseBody>?> RegistreerInschrijving(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<RegistreerUitschrijvingResponseBody>?> RegistreerUitschrijving(string kbonummer, MagdaCallReference reference);

    Task<ResponseEnvelope<AssociationRegistry.Integrations.Magda.Models.RegistreerInschrijving0200.RegistreerInschrijvingResponseBody>?> RegistreerInschrijvingPersoon(
        string insz,
        MagdaCallReference reference);
}
