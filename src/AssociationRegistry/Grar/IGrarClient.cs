namespace AssociationRegistry.Grar;

using Models;
using Models.PostalInfo;
using Vereniging;

public interface IGrarClient
{
    Task<AddressDetailResponse> GetAddressById(string adresId, CancellationToken cancellationToken);
    Task<AdresMatchResponseCollection> GetAddressMatches(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam, CancellationToken cancellationToken);
    Task<PostalInformationResponse> GetPostalInformation(string postcode);
}
