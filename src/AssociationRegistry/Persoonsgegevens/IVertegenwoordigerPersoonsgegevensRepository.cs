namespace AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensRepository
{
    Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens);
    Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId);
    Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refId);
}
