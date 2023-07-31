namespace AssociationRegistry.Magda;

using Models;
using Models.GeefOnderneming;
using Models.GeefOndernemingVKBO;

public interface IMagdaFacade
{
    Task<ResponseEnvelope<GeefOndernemingVKBOResponseBody>?> GeefOndernemingVKBO(string kbonummer, MagdaCallReference reference);
    Task<ResponseEnvelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, MagdaCallReference reference);
}
