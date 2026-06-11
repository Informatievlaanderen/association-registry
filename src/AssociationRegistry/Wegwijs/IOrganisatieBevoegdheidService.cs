namespace AssociationRegistry.Wegwijs;

public interface IOrganisatieBevoegdheidService
{
    Task<string[]> IsGemachtigdeOrganisatie(string initiator, string geregistreerdDoor);
}
