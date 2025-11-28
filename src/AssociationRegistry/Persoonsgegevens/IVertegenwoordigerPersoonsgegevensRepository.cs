namespace AssociationRegistry.Persoonsgegevens;

using DecentraalBeheer.Vereniging;

public interface IVertegenwoordigerPersoonsgegevensRepository
{
    Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens);
    Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId, CancellationToken cancellationToken);
    Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refId, CancellationToken cancellationToken);
    void Delete(VCode vCode, int vertegenwoordigerId);
}
