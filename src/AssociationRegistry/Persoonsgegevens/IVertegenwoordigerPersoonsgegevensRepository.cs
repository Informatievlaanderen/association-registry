namespace AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensRepository
{
    Task Save(VertegenwoordigerPersoonsgegevens vertegenwoordigerPersoonsgegevens);
}
