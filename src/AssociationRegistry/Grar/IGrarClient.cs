namespace AssociationRegistry.Grar;

using Models;

public interface IGrarClient
{
    Task<IReadOnlyCollection<AddressMatchResponse>> GetAddressMatches(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam);
    Task<PostalInformationResponse> GetPostalInformation(string postcode);
    Task<AddressDetailResponse> GetAddress(string adresId);
}
