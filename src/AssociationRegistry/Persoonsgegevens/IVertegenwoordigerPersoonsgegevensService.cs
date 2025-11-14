namespace AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensService
{
    Task<VertegenwoordigerPersoonsgegevens> Get(Guid refId);
    Task<VertegenwoordigerPersoonsgegevens[]> Get(Guid[] refIds);
}
