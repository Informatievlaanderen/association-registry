namespace AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensRepository
{
    Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens);
    Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId, CancellationToken cancellationToken);
    Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refId, CancellationToken cancellationToken);
}
