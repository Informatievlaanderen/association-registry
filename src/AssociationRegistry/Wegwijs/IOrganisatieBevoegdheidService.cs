namespace AssociationRegistry.Wegwijs;

public interface IOrganisatieBevoegdheidService
{
    Task<string[]> GetAndValidateGemachtigdeOrganisaties(string initiator, string geregistreerdDoor);
}
