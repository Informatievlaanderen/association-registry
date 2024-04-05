namespace AssociationRegistry.Grar;

using Models;

public interface IGrarClient
{
    Task<IReadOnlyCollection<AddressMatch>> GetAddress(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam);
}
