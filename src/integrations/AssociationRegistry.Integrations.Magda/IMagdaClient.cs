namespace AssociationRegistry.Integrations.Magda;

using Models;
using Models.GeefOnderneming;
using Models.GeefOndernemingVKBO;
using Models.RegistreerInschrijving;
using Models.RegistreerUitschrijving;

public interface IMagdaClient
{
    Task<ResponseEnvelope<GeefOndernemingVKBOResponseBody>?> GeefOndernemingVKBO(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<RegistreerInschrijvingResponseBody>?> RegistreerInschrijving(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<RegistreerUitschrijvingResponseBody>?> RegistreerUitschrijving(string kbonummer, MagdaCallReference reference);
}
