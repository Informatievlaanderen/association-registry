namespace AssociationRegistry.Wegwijs;

public interface IOrganisatieBevoegdheidService
{
    Task<string[]> GetGemachtigdeOrganisaties(string initiator, string geregistreerdDoor);
}
