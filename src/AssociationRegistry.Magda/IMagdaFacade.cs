namespace AssociationRegistry.Magda;

using Models;

public interface IMagdaFacade
{
    Task<Envelope<GeefOndernemingResponseBody>?> GeefOnderneming(string kbonummer, MagdaCallReference reference);
}
