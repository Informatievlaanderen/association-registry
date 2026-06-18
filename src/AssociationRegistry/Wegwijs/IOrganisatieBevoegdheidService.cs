namespace AssociationRegistry.Wegwijs;

public interface IOrganisatieBevoegdheidService
{
    Task<string[]> GetOpvolgers(string initiator);
}
