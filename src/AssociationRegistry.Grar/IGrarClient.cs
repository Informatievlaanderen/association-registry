namespace AssociationRegistry.Grar;

public interface IGrarClient
{
    Task GetAddress(string gemeentenaam, string straatnaam, string huisNummer);
}
